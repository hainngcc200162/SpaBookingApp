using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpaBookingApp.Dtos.Category;
using SpaBookingApp.Services.CategoryService;
using System.Threading.Tasks;

namespace SpaBookingApp.Pages.Categories
{
    public class CategoryModel : PageModel
    {
        private readonly ICategoryService _categoryService;
        private readonly HttpClient _httpClient;

        public CategoryModel(HttpClient httpClient, ICategoryService categoryService)
        {
            _categoryService = categoryService;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://fspa.azurewebsites.net/");
        }

        public List<GetCategoryDto> Categories { get; set; }
        public PageInformation PageInformation { get; set; }
        public string ErrorMessage { get; set; }

        [BindProperty(SupportsGet = true)]
        public string searchByName { get; set; } // Property to bind to search input

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 0; // Property to bind to page index

        public async Task OnGetAsync()
        {
            var serviceResponse = await _categoryService.GetAllCategories(searchByName, PageIndex);
            if (serviceResponse.Success)
            {
                Categories = serviceResponse.Data;
                PageInformation = serviceResponse.PageInformation;
            }
            else
            {
                ErrorMessage = serviceResponse.Message;
            }
        }

        public async Task<IActionResult> OnGetShowCategoryDetails(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Category/{id}");
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<GetCategoryDto>>();
                if (result.Success)
                {
                    TempData["Category"] = result.Data; // Pass the category data to the next request using TempData
                    return RedirectToPage("CategoryDetails"); // Assuming you have a "CategoryDetails.cshtml" page to display the category details
                }
                else
                {
                    ErrorMessage = result.Message;
                    return RedirectToPage("Index"); // Redirect to the index page or another suitable page
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return RedirectToPage("Index"); // Handle the exception and redirect appropriately
            }
        }

        public async Task<IActionResult> OnPostUpdateAsync(int id, UpdateCategoryDto updateCategoryDto)
        {
            updateCategoryDto.Id = id; // Set the ID of the category to be updated
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/Category/{id}", updateCategoryDto);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<List<GetCategoryDto>>>();
                if (result.Success)
                {
                    return RedirectToPage("Index"); // Redirect to the index page or another suitable page
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

            // If the update fails or an exception occurs, return the "Index" page with the current model state
            return Page();
        }


        public async Task<IActionResult> OnGetDeleteCategoryAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Category/{id}");
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<GetCategoryDto>>();
                if (result.Success)
                {
                    return RedirectToPage("DeleteCategory", new { id = result.Data.Id });
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                    return RedirectToPage("/Categories/Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToPage("/Categories/Index"); // Handle the exception and redirect appropriately
            }
        }
    }
}
