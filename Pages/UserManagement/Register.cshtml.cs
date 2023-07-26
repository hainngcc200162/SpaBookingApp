using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SpaBookingApp.Dtos.User;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpaBookingApp.Pages.UserManagement
{
    public class RegisterModel : PageModel
    {
        private readonly ILogger<RegisterModel> _logger;
        private readonly HttpClient _httpClient;

        [BindProperty]
        public UserRegisterDto UserRegisterDto { get; set; }

        public string ErrorMessage { get; set; }

        public RegisterModel(ILogger<RegisterModel> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5119/"); // Cập nhật đúng địa chỉ của API Register
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

            var response = await _httpClient.PostAsJsonAsync("auth/register", UserRegisterDto);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<int>>();
                if (result.Success)
                {
                    // Xử lý khi đăng ký thành công
                    return RedirectToPage("Login");
                }
                else
                {
                    // Hiển thị thông báo lỗi
                    ErrorMessage = result.Message;
                }
            }
            else
            {
                // Xử lý khi có lỗi không liên quan đến API đăng ký
                ErrorMessage = "An error occurred while processing the request.";
            }

            return Page();
        }

    }
}
