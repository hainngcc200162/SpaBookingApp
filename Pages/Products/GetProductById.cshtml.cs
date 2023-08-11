using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SpaBookingApp.Dtos.SpaProduct;

namespace SpaBookingApp.Pages.Products
{
    public class GetProductByIdModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public GetSpaProductDto Product { get; set; }
        public string ErrorMessage { get; set; }

        public GetProductByIdModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5119/"); // Thay thế bằng URL cơ sở của API của bạn
        }

        public async Task<IActionResult> OnGet(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/SpaProduct/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<ServiceResponse<GetSpaProductDto>>();
                    Product = data.Data;
                }
                else
                {
                    ErrorMessage = "Error retrieving SpaProduct details.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            return Page();
        }
    }
}
