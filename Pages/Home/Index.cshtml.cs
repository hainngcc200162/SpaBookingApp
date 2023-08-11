using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpaBookingApp.Dtos.SpaProduct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SpaBookingApp.Pages.Home
{
    public class IndexModel : PageModel
    {
        private readonly ISpaProductService _spaproductService;
        private readonly HttpClient _httpClient;

        public IndexModel(ISpaProductService spaproductService, HttpClient httpClient)
        {
            _spaproductService = spaproductService;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5119/"); // Update to your API URL
        }

        public List<GetSpaProductDto> SpaProducts { get; set; }
        public PageInformation PageInformation { get; set; }
        public string ErrorMessage { get; set; }
        public List<GetCategoryDto> Categories { get; set; }



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


        private async Task LoadCategories()
        {
            var categoriesResponse = await _httpClient.GetFromJsonAsync<ServiceResponse<List<GetCategoryDto>>>("api/Category/GetAll");
            Categories = categoriesResponse?.Data;
        }
    }
}
