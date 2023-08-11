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

        public ProductModel(ISpaProductService spaproductService, IHttpClientFactory httpClientFactory)
        {
            _spaproductService = spaproductService;
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("http://localhost:5119/");
        }

        public List<GetSpaProductDto> SpaProducts { get; set; }
        public PageInformation PageInformation { get; set; }
        public string ErrorMessage { get; set; }
        public List<GetCategoryDto> Categories { get; set; }

        // Properties to bind to query parameters
        [BindProperty(SupportsGet = true)]
        public string Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Category { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? MinPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? MaxPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortBy { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; }

        public async Task OnGetAsync(int? pageIndex)
        {
            // Check if pageIndex is null, and if so, set it to 0
            if (!pageIndex.HasValue)
            {
                pageIndex = 0;
            }

            await LoadCategories();

            var response = await _spaproductService.GetAllProducts(Search, Category, MinPrice, MaxPrice, SortBy, SortOrder, pageIndex.Value);

            if (response.Success)
            {
                SpaProducts = response.Data;
                PageInformation = response.PageInformation;
            }
            else
            {
                ErrorMessage = response.Message;
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

        private async Task LoadCategories()
        {
            var categoriesResponse = await _httpClient.GetFromJsonAsync<ServiceResponse<List<GetCategoryDto>>>("api/Category/GetAll");
            Categories = categoriesResponse?.Data;
        }
    }
}
