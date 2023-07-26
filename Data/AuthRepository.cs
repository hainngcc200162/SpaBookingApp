using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using SpaBookingApp.Dtos.User;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace SpaBookingApp.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;


        public AuthRepository(DataContext context, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }


        public async Task<ServiceResponse<string>> Login(string email, string password)
        {
            var response = new ServiceResponse<string>();
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower().Equals(email.ToLower()));

            if (user is null)
            {
                response.Success = false;
                response.Message = "User not found.";
            }
            else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Wrong password.";
            }
            else
            {
                response.Data = CreateToken(user);
                response.Message = "Login successful.";

                // Send the login email here using the _emailService asynchronously
                Task.Run(async () =>
                {
                    try
                    {
                        MailRequest mailrequest = new MailRequest();
                        mailrequest.ToEmail = user.Email;
                        mailrequest.Subject = "Thông tin đăng nhập";
                        mailrequest.Body = $"Xin chào {user.FirstName} {user.LastName},\nBạn đã đăng nhập thành công vào lúc {DateTime.Now}.";
                        await _emailService.SendEmailAsync(mailrequest);
                    }
                    catch (Exception ex)
                    {
                        // Handle any exceptions that may occur while sending the email
                        // You might want to log the error or perform other actions
                    }
                });
            }
            return response;
        }



        public async Task<ServiceResponse<int>> Register(User user, string password, UserRole role, string phoneNumber, string confirmPassword)
        {
            var response = new ServiceResponse<int>();

            if (await UserExists(user.Email))
            {
                response.Success = false;
                response.Message = "User with this email already exists.";
                return response;
            }
            else if (password != confirmPassword)
            {
                response.Success = false;
                response.Message = "Passwords do not match.";
                return response;
            }

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.Role = role;
            user.PhoneNumber = phoneNumber;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            response.Data = user.Id;
            response.Message = "Registration successful";
            return response;
        }

        public async Task<bool> UserExists(string email)
        {
            if (await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower()))
            {
                return true;
            }
            return false;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var appSettingsToken = _configuration.GetSection("AppSettings:Token").Value;
            if (appSettingsToken is null)
                throw new Exception("AppSettings Token is null !!");

            SymmetricSecurityKey key = new SymmetricSecurityKey(System.Text.Encoding.UTF8
                .GetBytes(appSettingsToken));

            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
        public async Task<ServiceResponse<bool>> ChangePassword(string email, string oldPassword, string newPassword, string confirmNewPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return new ServiceResponse<bool> { Success = false, Message = "Invalid Email." };
            }

            // Kiểm tra mật khẩu cũ
            if (!VerifyPasswordHash(oldPassword, user.PasswordHash, user.PasswordSalt))
            {
                return new ServiceResponse<bool> { Success = false, Message = "Incorrect old password." };
            }

            // Kiểm tra xác nhận mật khẩu mới
            if (newPassword != confirmNewPassword)
            {
                return new ServiceResponse<bool> { Success = false, Message = "Passwords do not match." };
            }

            // Tạo mới salt và hash cho mật khẩu mới
            CreatePasswordHash(newPassword, out byte[] newPasswordHash, out byte[] newPasswordSalt);

            // Cập nhật mật khẩu mới cho user
            user.PasswordHash = newPasswordHash;
            user.PasswordSalt = newPasswordSalt;

            // Lưu thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool> { Success = true, Data = true, Message = "Password changed successfully." };
        }

        public async Task<ServiceResponse<bool>> ResetPassword(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return new ServiceResponse<bool> { Success = false, Message = "Invalid Email." };
            }

            // Tạo mật khẩu mới ngẫu nhiên
            string newPassword = GenerateRandomPassword(); // Implement this method to generate a random password

            // Tạo salt và hash cho mật khẩu mới
            CreatePasswordHash(newPassword, out byte[] newPasswordHash, out byte[] newPasswordSalt);

            // Cập nhật mật khẩu mới cho người dùng
            user.PasswordHash = newPasswordHash;
            user.PasswordSalt = newPasswordSalt;

            // Lưu thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            // Send the login email here using the _emailService asynchronously
            Task.Run(async () =>
            {
                try
                {
                    // Gửi email chứa mật khẩu mới cho người dùng
                    MailRequest mailrequest = new MailRequest();
                    mailrequest.ToEmail = email;
                    mailrequest.Subject = "Mật khẩu mới đã được đặt lại";
                    mailrequest.Body = $"Xin chào,\n\nMật khẩu mới của bạn là: \"{newPassword}\"\n\nVui lòng cập nhật mật khẩu mới hoặc đăng nhập bằng mật khẩu mới này."; await _emailService.SendEmailAsync(mailrequest);
                }
                catch (Exception ex)
                {
                    // Xử lý ngoại lệ nếu xảy ra lỗi khi gửi email
                    // Bạn có thể ghi log lỗi hoặc thực hiện các hành động khác
                }
            });

            return new ServiceResponse<bool> { Success = true, Data = true, Message = "Password reset successfully." };
        }

        private string GenerateRandomPassword(int length = 12)
        {
            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+";

            StringBuilder sb = new StringBuilder();
            Random random = new Random();

            while (0 < length--)
            {
                sb.Append(validChars[random.Next(validChars.Length)]);
            }

            return sb.ToString();
        }
        public async Task SeedAdminUser()
        {
            var adminEmail = "admin@gmail.com";
            var adminExists = await UserExists(adminEmail);
            if (!adminExists)
            {
                var adminUser = new User
                {
                    FirstName = "Admin",
                    LastName = "User",
                    Email = adminEmail,
                    Role = UserRole.Admin,
                    PhoneNumber = "0123456789"
                };

                string adminPassword = "admin123";
                CreatePasswordHash(adminPassword, out byte[] passwordHash, out byte[] passwordSalt);

                adminUser.PasswordHash = passwordHash;
                adminUser.PasswordSalt = passwordSalt;

                _context.Users.Add(adminUser);
                await _context.SaveChangesAsync();
            }
        }

    }
}