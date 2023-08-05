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
        private readonly IAuthRepository _authRepo; // Thay thế IAuthRepository bằng interface tương ứng
        public string Message { get; set; }
        public string AlertClass { get; set; }

        public LoginModel(IAuthRepository authRepo)
        {
            _authRepo = authRepo;
        }

        public void OnGet()
        {
            // Không cần thực hiện gì ở đây
        }

        public async Task<IActionResult> OnPostAsync(UserLoginDto request)
        {
            var response = await _authRepo.Login(request);

            if (response.Success)
            {
                return new JsonResult(new { Success = true });
            }
            else
            {
                return new JsonResult(new { Success = false, Message = response.Message });
            }
        }

    }
}