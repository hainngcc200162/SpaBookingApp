using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpaBookingApp.Dtos.User;


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
            var response = await _authRepo.Login(request.Email, request.Password);
            if (response.Success)
            {
                // Đăng nhập thành công, gửi email thông báo
                var emailDto = new EmailDto
                {
                    To = request.Email,
                    Subject = "Đăng nhập thành công",
                    Body = "Xin chào, bạn đã đăng nhập thành công vào hệ thống của chúng tôi!"
                };

                _emailService.SendEmail(emailDto); // Truyền đối tượng EmailDto vào phương thức SendEmail

                return Ok(response); // Trả về Ok nếu đăng nhập thành công
            }
            return BadRequest(response);
        }


        [HttpPost("ChangePassword")]
        public async Task<ActionResult<ServiceResponse<bool>>> ChangePassword(UserChangePasswordDto request)
        {
            var response = await _authRepo.ChangePassword(request.Username, request.OldPassword, request.NewPassword);
            if (!response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }



    }
}
