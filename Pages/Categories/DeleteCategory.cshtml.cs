using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpaBookingApp.Dtos.Category;
using SpaBookingApp.Services.CategoryService;
using System.Threading.Tasks;

namespace SpaBookingApp.Pages.Categories
{
    public class DeleteCategoryModel : PageModel
    {
        private readonly ICategoryService _categoryService;

        public DeleteCategoryModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [BindProperty]
        public GetCategoryDto Category { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var serviceResponse = await _categoryService.GetCategoryById(id);
            if (serviceResponse.Success)
            {
                Category = serviceResponse.Data;
                return Page();
            }
            else
            {
                ErrorMessage = serviceResponse.Message;
                return RedirectToPage("/Categories/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var serviceResponse = await _categoryService.DeleteCategory(id);
            if (serviceResponse.Success)
            {
                return RedirectToPage("/Categories/Index");
            }
            else
            {
                ErrorMessage = serviceResponse.Message;
                return RedirectToPage("/Categories/Index");
            }
        }
    }
}
