using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SpaBookingApp.Dtos.Category;
using SpaBookingApp.Services.CategoryService;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SpaBookingApp.Pages.Categories
{
    public class UpdateCategoryModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ICategoryService _categoryService;

        [BindProperty]
        public UpdateCategoryDto Category { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public UpdateCategoryModel(HttpClient httpClient, ICategoryService categoryService)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5119/");
            _categoryService = categoryService;
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
                var response = await _categoryService.UpdateCategory(Category);
                if (response.Data is not null && response.Success)
                {
                    return RedirectToPage("/Categories/Index");
                }
                else if (response is not null)
                {
                    ErrorMessage = response.Message;
                }
                else
                {
                    ErrorMessage = "An error occurred while processing the request.";
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
