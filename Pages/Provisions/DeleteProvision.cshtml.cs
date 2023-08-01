// Trong Razor Page
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpaBookingApp.Dtos.Provision;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpaBookingApp.Pages.Provisions
{
    public class DeleteProvisionModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DeleteProvisionModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5119/"); // Thay thế bằng URL cơ sở của API delete provision
        }

        [BindProperty]
        public GetProvisionDto Provision { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/Provision/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<ServiceResponse<GetProvisionDto>>();
                    Provision = data.Data;
                    return Page();
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

            return RedirectToPage("Index");
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/api/Provision/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("Index");
                }
                else
                {
                    ErrorMessage = "Error deleting provision.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            return RedirectToPage("Index");
        }
    }
}
