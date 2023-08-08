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

        public bool? IsVerified { get; set; }

        

        public VerifyAccountModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5119/");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string email)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/verifyaccount", new AccountVerificationDto { Email = email, VerificationCode = VerificationCode });

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
                if (result != null && result.Success && result.Data)
                {
                    IsVerified = true;
                    return RedirectToPage("/UserManagement/Login");
                }
                else
                {
                    IsVerified = false;
                    ModelState.AddModelError("VerificationCode", "Invalid verification code.");
                }
            }
            else
            {
                IsVerified = false;
                ModelState.AddModelError("VerificationCode", "An error occurred while verifying the account.");
            }

            return Page();
        }
    }
}
