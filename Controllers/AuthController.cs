using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpaBookingApp.Dtos.User;
using SpaBookingApp.Helpter;
using SpaBookingApp.Services.EmailService;

namespace SpaBookingApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public AuthController(IAuthRepository authRepo, IMapper mapper, IEmailService emailService)
        {
            _authRepo = authRepo;
            _mapper = mapper;
            _emailService = emailService;
        }

        [HttpPost("SendMail")]
        public async Task<IActionResult> SendMail()
        {
            try
            {
                MailRequest mailrequest = new MailRequest();
                mailrequest.ToEmail = "hainngcc200162@fpt.edu.vn";
                mailrequest.Subject = "Welcome to NGOCHAI";
                mailrequest.Body = "Thank You For All !!!!";
                await _emailService.SendEmailAsync(mailrequest);
                return Ok();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("Register")]
        public async Task<ActionResult<ServiceResponse<int>>> Register(UserRegisterDto request)
        {
            var response = await _authRepo.Register(
                new User
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    Role = UserRole.Customer // You can update the default role here if needed
                },
                request.Password, // Pass the password argument here
                UserRole.Customer,
                request.PhoneNumber,
                request.ConfirmPassword
            );

            if (response.Success)
            {
                return Ok(response); // Return Ok if registration is successful
            }

            return BadRequest(response);
        }


        [HttpPost("Login")]
        public async Task<ActionResult<ServiceResponse<int>>> Login(UserLoginDto request)
        {
            var response = await _authRepo.Login(request.Email, request.Password, request.IsVerified);
            if (response.Success)
            {
                return Ok(response); // Trả về Ok nếu đăng nhập thành công
            }
            return BadRequest(response);
        }

        [HttpPost("ChangePassword")]
        public async Task<ActionResult<ServiceResponse<bool>>> ChangePassword(UserChangePasswordDto request)
        {
            var response = await _authRepo.ChangePassword(request.Email, request.OldPassword, request.NewPassword, request.ConfirmNewPassword);
            if (!response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ForgotPasswordDto request)
        {
            var response = await _authRepo.ResetPassword(request.Email);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpPost("VerifyAccount")]
        public async Task<IActionResult> VerifyAccount(AccountVerificationDto verificationDto)
        {
            try
            {
                var response = await _authRepo.VerifyAccount(verificationDto.Email, verificationDto.VerificationCode);

                if (response.Success)
                {
                    return Ok(response);
                }

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Something went wrong while verifying the account." });
            }
        }

        [Authorize]
        [HttpGet("Profile")]
        public async Task<IActionResult> GetProfile()
        {
            int userId = JwtReader.GetUserId(User);

            if (userId == 0)
            {
                return BadRequest(new ServiceResponse<UserProfileDto>
                {
                    Success = false,
                    Message = "Invalid user."
                });
            }

            var response = await _authRepo.GetProfile(userId);

            if (response.Success)
            {
                return Ok(response); // Return the user profile if retrieval is successful
            }

            return NotFound(response); // Return 404 Not Found if user not found
        }

        [Authorize]
        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile(UserProfileUpdateDto profileDto)
        {
            int userId = JwtReader.GetUserId(User);

            if (userId == 0)
            {
                return BadRequest(new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Invalid user."
                });
            }

            var response = await _authRepo.UpdateProfile(userId, profileDto);

            if (response.Success)
            {
                return Ok(response); // Return success response if update is successful
            }

            return NotFound(response); // Return 404 Not Found if user not found
        }

        [Authorize]
        [HttpDelete("DeleteAccount")]
        public async Task<IActionResult> DeleteAccount(UserDeleteDto deleteDto)
        {
            int userId = JwtReader.GetUserId(User);

            if (userId == 0)
            {
                return BadRequest(new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Invalid user."
                });
            }

            var response = await _authRepo.DeleteAccount(userId, deleteDto);

            if (!response.Success)
            {
                return BadRequest(response); // Return error message if deletion is not successful
            }

            return Ok(response); // Return success message if account deletion is successful
        }
    }
}
