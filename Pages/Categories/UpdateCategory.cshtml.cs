using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using SpaBookingApp.Dtos.Category;

namespace SpaBookingApp.Pages.Categories
{
    public class UpdateCategoryModel : PageModel
    {
        private readonly HttpClient _httpClient;

        [BindProperty]
        public UpdateCategoryDto Category { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public UpdateCategoryModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5119/");
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Category/{id}");
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<GetCategoryDto>>();
                if (result.Success)
                {
                    var getCategoryDto = result.Data;
                    Category = new UpdateCategoryDto
                    {
                        Id = getCategoryDto.Id,
                        Name = getCategoryDto.Name
                    };
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

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/Category/{Category.Id}", Category);
                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<GetCategoryDto>>();

                if (response.IsSuccessStatusCode && result.Success)
                {
                    SuccessMessage = "Category updated successfully.";
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

            // Ensure the error message is not empty
            if (string.IsNullOrEmpty(ErrorMessage))
            {
                ErrorMessage = "An error occurred while processing the request.";
            }

            return Page();
        }


    }
}
