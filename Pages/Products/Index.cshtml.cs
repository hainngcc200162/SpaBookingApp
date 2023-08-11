using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpaBookingApp.Dtos.SpaProduct;
using SpaBookingApp.Services.SpaProductService;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SpaBookingApp.Pages.Products
{
    public class ProductModel : PageModel
    {
        private readonly ISpaProductService _spaproductService;
        private readonly HttpClient _httpClient;

        public ProductModel(ISpaProductService spaproductService, HttpClient httpClient)
        {
            _spaproductService = spaproductService;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5119/");
        }

        public List<GetSpaProductDto> SpaProducts { get; set; }
        public PageInformation PageInformation { get; set; }
        public string ErrorMessage { get; set; }

        public async Task OnGetAsync(string search, string category, int? minPrice, int? maxPrice, string sortBy, string sortOrder, int? pageIndex)
        {
            // Check if pageIndex is null, and if so, set it to 0
            if (!pageIndex.HasValue)
            {
                pageIndex = 0;
            }

            var apiUrl = "api/spaproduct/GetAll"; // Update to the actual API endpoint
            var response = await _httpClient.GetAsync($"{apiUrl}?search={search}&category={category}&minPrice={minPrice}&maxPrice={maxPrice}&sortBy={sortBy}&sortOrder={sortOrder}&pageIndex={pageIndex}");

            if (response.IsSuccessStatusCode)
            {
                var productResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<List<GetSpaProductDto>>>();

                if (productResponse.Success)
                {
                    SpaProducts = productResponse.Data;
                    PageInformation = productResponse.PageInformation;
                }
                else
                {
                    ErrorMessage = productResponse.Message;
                }
            }
            else
            {
                ErrorMessage = "Unable to retrieve data from the API.";
            }
        }

        public async Task<IActionResult> OnGetShowProductDetails(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/spaproduct/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<ServiceResponse<GetSpaProductDto>>();
                    var product = data.Data;
                    TempData["Product"] = product;
                    return RedirectToPage("ProductDetails"); // Assuming you have a "ProductDetails.cshtml" page to display the product details
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


        public async Task<IActionResult> OnPostUpdateAsync(int id, UpdateSpaProductDto updateProductDto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/spaproduct/{id}", updateProductDto);
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("/"); // Redirect to the index page or another suitable page
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error updating product.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return Page();
        }
        public async Task<IActionResult> OnGetDeleteProductAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/SpaProduct/{id}");
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<GetSpaProductDto>>();
                if (result.Success)
                {
                    return RedirectToPage("DeleteProduct", new { id = result.Data.Id }); // Assuming you have a "DeleteProduct.cshtml" page
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                    return RedirectToPage("/Products/Index"); // Redirect to the index page or another suitable page
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToPage("/Products/Index"); // Redirect to the index page or another suitable page
            }
        }
    }
}
