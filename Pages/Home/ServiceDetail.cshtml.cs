using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace SpaBookingApp.Pages.Home
{
    public class GetProvisionByIdModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public GetProvisionDto Provision { get; set; }
        public string ErrorMessage { get; set; }

        public GetProvisionByIdModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://fspa.azurewebsites.net/"); // Thay thế bằng URL cơ sở của API của bạn
        }

        public async Task<IActionResult> OnGet(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/Provision/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<ServiceResponse<GetProvisionDto>>();
                    Provision = data.Data;
                }
                else
                {
                    ErrorMessage = "Error retrieving provision details.";
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