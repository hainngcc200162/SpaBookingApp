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

        public async Task<bool> UserExists(string email)
        {
            if (await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower()))
            {
                return true;
            }
            return false;
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
            user.IsVerified = false; // Set IsVerified to false for new users
            user.VerificationCode = GenerateVerificationCode(); // Generate a verification code for new users

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            response.Data = user.Id;
            response.Message = "Registration successful";
            // Send verification email asynchronously
            Task.Run(async () =>
            {
                try
                {
                    // Compose the email content
                    MailRequest mailRequest = new MailRequest
                    {
                        ToEmail = user.Email,
                        Subject = "Account Verification",
                        Body = $"Hello {user.FirstName} {user.LastName},\n\nPlease use the following verification code to confirm your account: \"{user.VerificationCode}\""
                    };

                    // Send the email
                    await _emailService.SendEmailAsync(mailRequest);
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that may occur while sending the email
                    // You might want to log the error or perform other actions
                }
            });

            return response;
        }

        public async Task<ServiceResponse<bool>> VerifyAccount(string email, string verificationCode)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower().Equals(email.ToLower()));
            if (user == null)
            {
                return new ServiceResponse<bool> { Success = false, Message = "User not found." };
            }

            if (user.IsVerified)
            {
                return new ServiceResponse<bool> { Success = false, Message = "Account is already verified." };
            }

            if (user.VerificationCode != verificationCode)
            {
                return new ServiceResponse<bool> { Success = false, Message = "Invalid verification code." };
            }

            user.IsVerified = true;
            user.VerificationCode = null; // Set VerificationCode to null to indicate account verification
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool> { Success = true, Data = true, Message = "Account verified successfully." };
        }

        public async Task<ServiceResponse<string>> Login(UserLoginDto request)
        {
            var response = new ServiceResponse<string>();
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower().Equals(request.Email.ToLower()));

            if (user is null)
            {
                response.Success = false;
                response.Message = "User not found.";
            }
            else if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Wrong password.";
            }
            else if (!user.IsVerified)
            {
                response.Success = false;
                response.Message = "Account is not verified. Please check your email for the verification code.";
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

        public async Task<ServiceResponse<bool>> ChangePassword(int userId, UserChangePasswordDto changepasswordDto)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return new ServiceResponse<bool> { Success = false, Message = "User not found." };
            }

            // Kiểm tra mật khẩu cũ
            if (!VerifyPasswordHash(changepasswordDto.OldPassword, user.PasswordHash, user.PasswordSalt))
            {
                return new ServiceResponse<bool> { Success = false, Message = "Incorrect old password." };
            }

            // Kiểm tra xác nhận mật khẩu mới
            if (changepasswordDto.NewPassword != changepasswordDto.ConfirmNewPassword)
            {
                return new ServiceResponse<bool> { Success = false, Message = "Passwords do not match." };
            }

            // Tạo mới salt và hash cho mật khẩu mới
            CreatePasswordHash(changepasswordDto.NewPassword, out byte[] newPasswordHash, out byte[] newPasswordSalt);

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

        public async Task<ServiceResponse<UserProfileDto>> GetProfile(int userId)
        {
            var response = new ServiceResponse<UserProfileDto>();
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found.";
                return response;
            }

            var userProfileDto = new UserProfileDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Role = user.Role
                // Map any other properties you want to expose in the user profile
            };

            response.Data = userProfileDto;
            response.Message = "User profile retrieved successfully.";

            return response;
        }

        public async Task<ServiceResponse<bool>> UpdateProfile(int userId, string password, UserProfileUpdateDto profileDto)
        {
            var response = new ServiceResponse<bool>();
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found.";
                return response;
            }

            // Verify the password
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Incorrect password.";
                return response;
            }

            // Cập nhật thông tin hồ sơ của người dùng với dữ liệu mới từ profileDto
            user.FirstName = profileDto.FirstName;
            user.LastName = profileDto.LastName;
            user.Email = profileDto.Email;
            user.PhoneNumber = profileDto.PhoneNumber;
            // Cập nhật các thông tin khác nếu cần

            // Lưu thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            response.Data = true;
            response.Message = "User profile updated successfully.";

            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteAccount(int userId, UserDeleteDto deleteDto)
        {
            var response = new ServiceResponse<bool>();
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found.";
                return response;
            }

            // Kiểm tra xác nhận mật khẩu
            if (deleteDto.Password != deleteDto.ConfirmPassword)
            {
                response.Success = false;
                response.Message = "Passwords do not match.";
                return response;
            }

            // Kiểm tra xác nhận mật khẩu
            if (!VerifyPasswordHash(deleteDto.Password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Incorrect password.";
                return response;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            response.Data = true;
            response.Message = "Account deleted successfully.";

            return response;
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
                    PhoneNumber = "0999999999",
                    Role = UserRole.Admin,
                    IsVerified = true,
                    VerificationCode = null
                };

                string adminPassword = "admin123";
                CreatePasswordHash(adminPassword, out byte[] passwordHash, out byte[] passwordSalt);

                adminUser.PasswordHash = passwordHash;
                adminUser.PasswordSalt = passwordSalt;

                _context.Users.Add(adminUser);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> DeleteUnverifiedAccounts()
        {
            DateTime currentDate = DateTime.Now;
            DateTime twentyMinutesAgo = currentDate.AddMinutes(-3);

            var unverifiedUsers = await _context.Users
                .Where(u => !u.IsVerified && u.CreatedAt < twentyMinutesAgo)
                .ToListAsync();

            _context.Users.RemoveRange(unverifiedUsers);
            return await _context.SaveChangesAsync();
        }

        //

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

            var httpContextAccessor = new HttpContextAccessor();
            httpContextAccessor.HttpContext.Items["Token"] = token;

            return tokenHandler.WriteToken(token);
        }


        private string GenerateRandomPassword(int length = 12)
        {
            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            StringBuilder sb = new StringBuilder();
            Random random = new Random();

            while (0 < length--)
            {
                sb.Append(validChars[random.Next(validChars.Length)]);
            }

            return sb.ToString();
        }

        private string GenerateVerificationCode()
        {
            const int codeLength = 6;
            const string allowedChars = "0123456789";

            Random random = new Random();
            string verificationCode = new string(Enumerable.Repeat(allowedChars, codeLength)
                                        .Select(s => s[random.Next(s.Length)])
                                        .ToArray());

            return verificationCode;
        }
    }
}