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
            _httpClient.BaseAddress = new Uri("http://localhost:5119/");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadCategories();
            return Page();
        }

        // {
        //     if (!ModelState.IsValid)
        //     {
        //         await LoadCategories();
        //         return Page();
        //     }
        //     try
        //     {
        //         var content = new MultipartFormDataContent();
        //         content.Add(new StringContent(SpaProduct.Name), "Name");
        //         content.Add(new StringContent(SpaProduct.Price.ToString()), "Price");
        //         content.Add(new StringContent(SpaProduct.QuantityInStock.ToString()), "QuantityInStock");
        //         content.Add(new StringContent(SpaProduct.Description), "Description");
        //         content.Add(new StringContent(SpaProduct.CategoryId.ToString()), "CategoryId");
        //         content.Add(new StreamContent(SpaProduct.Poster.OpenReadStream()), "Poster", SpaProduct.Poster.FileName);

        //         var response = await _httpClient.PostAsync("api/SpaProduct", content);
        //         response.EnsureSuccessStatusCode();

        //         var result = await response.Content.ReadFromJsonAsync<ServiceResponse<List<GetSpaProductDto>>>();
        //         if (result.Success)
        //         {
        //             // SuccessMessage = "Product created successfully.";
        //             // return RedirectToPage("/SpaProducts/Index");
        //         }
        //         else
        //         {
        //             ErrorMessage = result.Message;
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         ErrorMessage = ex.Message;
        //     }

        //     await LoadCategories();

        //     if (string.IsNullOrEmpty(ErrorMessage))
        //     {
        //         ErrorMessage = "An error occurred while processing the request.";
        //     }

        //     return Page();
        // }
        private async Task LoadCategories()
        {
            var categoriesResponse = await _httpClient.GetFromJsonAsync<ServiceResponse<List<GetCategoryDto>>>("api/Category/GetAll");
            Categories = categoriesResponse?.Data;
        }
    }
}
