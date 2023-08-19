using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace SpaBookingApp.Pages.Home
{
    public class GetCartsModel : PageModel
    {

        private readonly HttpClient _httpClient;

        public CartDto CartDto { get; set; }
        public string ErrorMessage { get; set; }

        public GetCartsModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5119/"); // Thay thế bằng URL cơ sở của API của bạn
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await FetchCart();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            return Page();
        }

        public async Task FetchCart()
        {
            var response = await _httpClient.GetAsync("/api/Cart");
            if (response.IsSuccessStatusCode)
            {
                CartDto = await response.Content.ReadFromJsonAsync<CartDto>();
            }
            else
            {
                ErrorMessage = "Error retrieving cart data.";
            }
        }

        public async Task UpdateCartApi(string productIdentifiers)
        {
            try
            {
                var content = new StringContent("", Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"/api/Cart/?productIdentifiers={productIdentifiers}", content);
                if (response.IsSuccessStatusCode)
                {
                    await FetchCart(); // Refresh the cart data after successful update
                }
                else
                {
                    ErrorMessage = "Error updating cart.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
    }
}