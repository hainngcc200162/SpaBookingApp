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

        public CategoryModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public List<GetCategoryDto> Categories { get; set; }
        public string ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            var serviceResponse = await _categoryService.GetAllCategories();
            if (serviceResponse.Success)
            {
                Categories = serviceResponse.Data;
            }
            else
            {
                ErrorMessage = serviceResponse.Message;
            }
        }

        public async Task<IActionResult> OnGetShowCategoryDetails(int id)
        {
            var serviceResponse = await _categoryService.GetCategoryById(id);
            if (serviceResponse.Success)
            {
                TempData["Category"] = serviceResponse.Data; // Pass the category data to the next request using TempData
                return RedirectToPage("CategoryDetails"); // Assuming you have a "CategoryDetails.cshtml" page to display the category details
            }
            else
            {
                ErrorMessage = serviceResponse.Message;
                return RedirectToPage("Index"); // Redirect to the index page or another suitable page
            }
        }

        public async Task<IActionResult> OnPostUpdateAsync(int id, UpdateCategoryDto updateCategoryDto)
        {
            updateCategoryDto.Id = id; // Set the ID of the category to be updated
            var serviceResponse = await _categoryService.UpdateCategory(updateCategoryDto);
            if (serviceResponse.Success)
            {
                return RedirectToPage("Index"); // Redirect to the index page or another suitable page
            }
            else
            {
                ErrorMessage = serviceResponse.Message;
                return Page(); // If the update fails, return the "Index" page with the current model state
            }
        }

        public async Task<IActionResult> OnGetDeleteCategoryAsync(int id)
        {
            var serviceResponse = await _categoryService.GetCategoryById(id);
            if (serviceResponse.Success)
            {
                return RedirectToPage("DeleteCategory", new { id = serviceResponse.Data.Id });
            }
            else
            {
                TempData["ErrorMessage"] = serviceResponse.Message;
                return RedirectToPage("/Categories/Index");
            }
        }
    }
}
