using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpaBookingApp.Dtos.User;
using System.Net.Http;

namespace SpaBookingApp.Pages
{
    public class VerifyAccountModel : PageModel
    {
        private readonly HttpClient _httpClient;

        [BindProperty]
        public string VerificationCode { get; set; }
        [BindProperty]
        public string Email { get; set; }

        public bool? IsVerified { get; set; }

        public VerifyAccountModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5119/");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var email = TempData["Email"] as string; // Lấy email từ TempData

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToPage("/Index"); // Nếu không tìm thấy email, chuyển hướng về trang chủ hoặc trang thông báo lỗi
            }

            var response = await _httpClient.PostAsJsonAsync("auth/verifyaccount", new AccountVerificationDto { Email = email, VerificationCode = VerificationCode });

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
                if (result != null && result.Success)
                {
                    IsVerified = result.Data;
                    return RedirectToPage("/UserManagement/Login");
                }
                else
                {
                    IsVerified = false;
                }
            }
            else
            {
                IsVerified = false;
            }

            return Page();
        }
    }
}
