using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpaBookingApp.Dtos.Category;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SpaBookingApp.Pages.Categories
{
    public class CreateCategoryModel : PageModel
    {
        private readonly HttpClient _httpClient;

        [BindProperty]
        public AddCategoryDto Category { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public CreateCategoryModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5119/");
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Category", Category);
                response.EnsureSuccessStatusCode(); // Throw an exception if the HTTP response is not successful

                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<List<GetCategoryDto>>>();
                if (result.Success)
                {
                    return RedirectToPage("/Categories/Index");
                }
                else
                {
                    ErrorMessage = result.Message;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            return Page();
        }
    }
}
