using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpaBookingApp.Dtos.Product;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpaBookingApp.Pages.Products
{
    public class DeleteProductModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DeleteProductModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5119/"); // Thay thế bằng URL cơ sở của API của bạn
        }

        [BindProperty]
        public GetProductDto Product { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/Product/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<ServiceResponse<GetProductDto>>();
                    Product = data.Data;
                    return Page();
                }
                else
                {
                    ErrorMessage = "Error retrieving product details.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            return RedirectToPage("Index");
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/api/Product/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("Index");
                }
                else
                {
                    ErrorMessage = "Error deleting product.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            return RedirectToPage("Index");
        }
    }
}
