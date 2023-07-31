using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SpaBookingApp.Dtos.Product;
using SpaBookingApp.Services.ProductService;
using Microsoft.AspNetCore.Mvc;

namespace SpaBookingApp.Pages.Products
{
    public class ProductModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IProductService _productService;
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }

        public ProductModel(HttpClient httpClient, IProductService productService)
        {
            _httpClient = httpClient;
            _productService = productService;
            _httpClient.BaseAddress = new Uri("http://localhost:5119/"); // Thay thế bằng URL cơ sở của API của bạn
        }

        public List<GetProductDto> Products { get; set; }
        public string ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            var response = await _httpClient.GetAsync("api/product/GetAll"); // Sử dụng một URI tương đối không có dấu gạch chéo đầu ("/api/product/GetAll" thay vì "/api/product/GetAll")
            if (response.IsSuccessStatusCode)
            {
                var serviceResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<List<GetProductDto>>>();
                if (serviceResponse != null && serviceResponse.Success)
                {
                    Products = serviceResponse.Data;
                }
                else
                {
                    ErrorMessage = serviceResponse?.Message ?? "Error";
                }
            }
            else
            {
                ErrorMessage = "";
            }
        }

        public async Task<IActionResult> OnGetShowProductDetails(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/Product/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<ServiceResponse<GetProductDto>>();
                    var product = data.Data;
                    TempData["Product"] = product; // Pass the product data to the next request using TempData
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

            return RedirectToPage("Index"); // Redirect to the index page or another suitable page
        }

        public async Task<IActionResult> OnPostUpdateAsync(int id, UpdateProductDto updateProductDto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/Product/{id}", updateProductDto);
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

            // If the update fails, return the "UpdateProduct" page with the current model state
            return Page();
        }

        public async Task<IActionResult> OnGetDeleteProductAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Product/{id}");
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<GetProductDto>>();
                if (result.Success)
                {
                    return RedirectToPage("DeleteProduct", new { id = result.Data.Id });
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                    return RedirectToPage("/Products/Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToPage("/Products/Index");
            }
        }

    }
}
