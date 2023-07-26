using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SpaBookingApp.Dtos.User;
using SpaBookingApp.Data;
using System.Threading.Tasks;

namespace SpaBookingApp.Pages.UserManagement
{
    public class LoginModel : PageModel
    {
        private readonly IAuthRepository _authRepository;
        private readonly ILogger<LoginModel> _logger;

        [BindProperty]
        public UserLoginDto UserLoginDto { get; set; }

        public string ErrorMessage { get; set; }

        public LoginModel(IAuthRepository authRepository, ILogger<LoginModel> logger)
        {
            _authRepository = authRepository;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var response = await _authRepository.Login(UserLoginDto.Email, UserLoginDto.Password);

            if (response.Success)
            {
                // var token = response.Data;

                // // Store the token in Local Storage
                // Response.Cookies.Append("accessToken", token, new Microsoft.AspNetCore.Http.CookieOptions
                // {
                //     HttpOnly = true,
                //     Secure = true,
                //     SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict
                // });

                // Handle successful login, e.g., redirect to the main page
                return RedirectToPage("/Categories/Index");
            }

            ErrorMessage = response.Message;

            return Page();
        }
    }
}
