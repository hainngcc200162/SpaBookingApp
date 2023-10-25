using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
            _httpClient.BaseAddress = new Uri("https://fspa.azurewebsites.net/"); // Thay thế bằng URL cơ sở của API của bạn
        }

        [BindProperty]
        public GetSpaProductDto Product { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/SpaProduct/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<ServiceResponse<GetSpaProductDto>>();
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

            return RedirectToPage("/Products/Index");
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/api/SpaProduct/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("/Products/Index");
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

            return RedirectToPage("/Products/Index");
        }
    }
}
