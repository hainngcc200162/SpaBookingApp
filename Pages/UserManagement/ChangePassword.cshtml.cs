using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpaBookingApp.Dtos.User;

namespace SpaBookingApp.Pages.UserManagement
{
    public class ChangePasswordModel : PageModel
    {
        private readonly HttpClient _httpClient;

        [BindProperty]
        public UserChangePasswordDto UserChangePasswordDto { get; set; }

        public string ErrorMessage { get; set; }

        public ChangePasswordModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5119/");
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

            var response = await _httpClient.PostAsJsonAsync("auth/changepassword", UserChangePasswordDto);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("Login");
            }

            var errorResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
            ErrorMessage = errorResponse?.Message ?? "An error occurred while processing the request.";

            return Page();
        }
    }
}
