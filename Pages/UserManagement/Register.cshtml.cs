using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SpaBookingApp.Dtos.User;
using SpaBookingApp.Data;
using System.Threading.Tasks;

namespace SpaBookingApp.Pages.UserManagement
{
    public class RegisterModel : PageModel
    {
        private readonly IAuthRepository _authRepository;
        private readonly ILogger<RegisterModel> _logger;

        [BindProperty]
        public UserRegisterDto UserRegisterDto { get; set; }

        public string ErrorMessage { get; set; }

        public RegisterModel(IAuthRepository authRepository, ILogger<RegisterModel> logger)
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

            var response = await _authRepository.Register(
                new User
                {
                    FirstName = UserRegisterDto.FirstName,
                    LastName = UserRegisterDto.LastName,
                    Email = UserRegisterDto.Email,
                    PhoneNumber = UserRegisterDto.PhoneNumber,
                    Role = UserRole.Customer // You can update the default role here if needed
                },
                UserRegisterDto.Password,
                UserRole.Customer,
                UserRegisterDto.PhoneNumber,
                UserRegisterDto.ConfirmPassword
            );

            if (response.Success)
            {
                return RedirectToPage("Login");
            }

            ErrorMessage = response.Message;
            return Page();
        }
    }
}
