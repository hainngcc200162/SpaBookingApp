using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpaBookingApp.Dtos.SpaProduct;
using SpaBookingApp.Dtos.Category;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SpaBookingApp.Pages.Products
{
    public class CreateProductModel : PageModel
    {
        private readonly HttpClient _httpClient;

        [BindProperty]
        public AddSpaProductDto SpaProduct { get; set; }

        public List<GetCategoryDto> Categories { get; set; }

        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        public CreateProductModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://fspa.azurewebsites.net/");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadCategories();
            return Page();
        }

        private async Task LoadCategories()
        {
            var categoriesResponse = await _httpClient.GetFromJsonAsync<ServiceResponse<List<GetCategoryDto>>>("api/Category/GetAll");
            Categories = categoriesResponse?.Data;
        }
    }
}
