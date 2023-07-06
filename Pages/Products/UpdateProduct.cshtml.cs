using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SpaBookingApp.Dtos.Product;
using SpaBookingApp.Dtos.Category;
using SpaBookingApp.Services.ProductService;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace SpaBookingApp.Pages.Products
{
    public class UpdateProductModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IProductService _productService;

        [BindProperty]
        public UpdateProductDto Product { get; set; }

        public List<SelectListItem> Categories { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public UpdateProductModel(HttpClient httpClient, IProductService productService)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5119/");
            _productService = productService;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Product/{id}");
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<GetProductDto>>();
                if (result.Success)
                {
                    var getProductDto = result.Data;
                    Product = new UpdateProductDto
                    {
                        Id = getProductDto.Id,
                        Name = getProductDto.Name,
                        Price = getProductDto.Price,
                        QuantityInStock = getProductDto.QuantityInStock,
                        CategoryId = int.Parse(getProductDto.CategoryId),
                        Description = getProductDto.Description,
                        PosterName = getProductDto.PosterName
                    };
                    await LoadCategories();
                    return Page();
                }
                else
                {
                    ErrorMessage = result.Message;
                    return RedirectToPage("/Products/Index");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return RedirectToPage("/Products/Index");
            }
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (Product.Poster == null) // Check if a new file is selected by the user
                {
                    // If no new file is selected, retain the old value of Poster
                    var getProductResponse = await _productService.GetProductById(Product.Id);
                    if (getProductResponse.Data != null && getProductResponse.Success)
                    {
                        Product.PosterName = getProductResponse.Data.PosterName;
                    }
                }

                var response = await _productService.UpdateProduct(Product);
                if (response.Data is not null && response.Success)
                {
                    return RedirectToPage("/Products/Index");
                }
                else if (response is not null)
                {
                    ErrorMessage = response.Message;
                }
                else
                {
                    ErrorMessage = "An error occurred while processing the request.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            // Ensure the error message is not empty
            if (string.IsNullOrEmpty(ErrorMessage))
            {
                ErrorMessage = "An error occurred while processing the request.";
            }

            await LoadCategories();
            return Page();
        }

        public async Task LoadCategories()
        {
            var response = await _httpClient.GetAsync("api/Category/GetAll");
            if (response.IsSuccessStatusCode)
            {
                var categories = await response.Content.ReadFromJsonAsync<ServiceResponse<List<GetCategoryDto>>>();
                Categories = categories.Data
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                    .ToList();
            }
            else
            {
                ErrorMessage = "Failed to load categories.";
            }
        }
    }
}
