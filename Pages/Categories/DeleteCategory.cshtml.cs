using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpaBookingApp.Dtos.Category;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpaBookingApp.Pages.Categories
{
    public class DeleteCategoryModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DeleteCategoryModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://fspa.azurewebsites.net/"); // Update the base address as needed
        }

        [BindProperty]
        public GetCategoryDto Category { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Category/{id}");
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<GetCategoryDto>>();
                if (result.Success)
                {
                    Category = result.Data;
                    return Page();
                }
                else
                {
                    ErrorMessage = result.Message;
                    return RedirectToPage("/Categories/Index");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return RedirectToPage("/Categories/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Category/{id}");
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<List<GetCategoryDto>>>();
                if (result.Success)
                {
                    return RedirectToPage("/Categories/Index");
                }
                else
                {
                    ErrorMessage = result.Message;
                    return RedirectToPage("/Categories/Index");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return RedirectToPage("/Categories/Index");
            }
        }
    }
}
