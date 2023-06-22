using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpaBookingApp.Dtos.Product;
using SpaBookingApp.Services.ProductService;
using System;
using System.Threading.Tasks;

namespace SpaBookingApp.Pages.Products
{
    public class DeleteProductModel : PageModel
    {
        private readonly IProductService _productService;

        public DeleteProductModel(IProductService productService)
        {
            _productService = productService;
        }

        [BindProperty]
        public GetProductDto Product { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var serviceResponse = await _productService.GetProductById(id);
            if (serviceResponse.Success)
            {
                Product = serviceResponse.Data;
                return Page();
            }
            else
            {
                ErrorMessage = serviceResponse.Message;
                return RedirectToPage("Index");
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var serviceResponse = await _productService.DeleteProduct(id);
            if (serviceResponse.Success)
            {
                return RedirectToPage("Index");
            }
            else
            {
                ErrorMessage = serviceResponse.Message;
                return RedirectToPage("Index");
            }
        }
    }
}
