using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SpaBookingApp.Dtos.SpaProduct;
using SpaBookingApp.Dtos.Category;
using SpaBookingApp.Services.SpaProductService;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace SpaBookingApp.Pages.Products
{
    public class UpdateProductModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ISpaProductService _spaproductService;

        [BindProperty]
        public UpdateSpaProductDto Product { get; set; }

        public List<SelectListItem> Categories { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public UpdateProductModel(HttpClient httpClient, ISpaProductService spaproductService)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5119/");
            _spaproductService = spaproductService;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/SpaProduct/{id}");
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<GetSpaProductDto>>();
                if (result.Success)
                {
                    var getProductDto = result.Data;
                    Product = new UpdateSpaProductDto
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
                    return RedirectToPage("/SpaProducts/Index");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return RedirectToPage("/SpaProducts/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (Product.Poster == null)
                {
                    var getProductResponse = await _spaproductService.GetProductById(Product.Id);
                    if (getProductResponse.Data != null && getProductResponse.Success)
                    {
                        Product.PosterName = getProductResponse.Data.PosterName;
                    }
                }

                // Chuyển đổi Product thành JSON và gửi đi
                var response = await _httpClient.PutAsJsonAsync($"api/SpaProduct/{Product.Id}", Product);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<List<GetSpaProductDto>>>();
                if (result.Success)
                {
                    return RedirectToPage("/SpaProducts/Index");
                }
                else
                {
                    ErrorMessage = result.Message;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

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
