using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpaBookingApp.Dtos.Category;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SpaBookingApp.Pages.Categories
{
    public class GetCategoryByIdModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public GetCategoryDto Category { get; set; }

        public string ErrorMessage { get; set; }

        public GetCategoryByIdModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5119/");
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<GetCategoryDto>>($"api/Category/{id}");
            Category = response?.Data;

            if (Category == null)
            {
                ErrorMessage = response?.Message ?? "Category not found";
                return Page();
            }

            return Page();
        }
    }
}
