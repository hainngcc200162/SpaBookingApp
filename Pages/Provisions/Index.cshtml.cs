using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace SpaBookingApp.Pages.Provisions 
{
    public class ProvisionModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IProvisionService _provisionService;

        public ProvisionModel(HttpClient httpClient, IProvisionService provisionService)
        {
            _httpClient = httpClient;
            _provisionService = provisionService;
            _httpClient.BaseAddress = new Uri("http://localhost:5119/"); // Thay thế bằng URL cơ sở của API của bạn
        }

        public List<GetProvisionDto> Provisions { get; set; }
        public string ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            var response = await _httpClient.GetAsync("api/provision/GetAll");
            if (response.IsSuccessStatusCode)
            {
                var serviceResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<List<GetProvisionDto>>>();
                if (serviceResponse != null && serviceResponse.Success)
                {
                    Provisions = serviceResponse.Data; 
                }
                else
                {
                    ErrorMessage = serviceResponse?.Message ?? "Error";
                }
            }
            else
            {
                ErrorMessage = "Can not connect to API.!";
            }
        }

        public async Task<IActionResult> OnGetShowProvisionDetails(int id) 
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/provision/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<ServiceResponse<GetProvisionDto>>();
                    var provision = data.Data; 
                    TempData["Provision"] = provision; // Pass the provision data to the next request using TempData
                    return RedirectToPage("ProvisionDetails"); // Assuming you have a "ProvisionDetails.cshtml" page to display the provision details
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

            return RedirectToPage("Index"); // Redirect to the index page or another suitable page
        }

        public async Task<IActionResult> OnPostUpdateAsync(int id, UpdateProvisionDto updateProvisionDto) 
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/provision/{id}", updateProvisionDto); 
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("/"); // Redirect to the index page or another suitable page
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error updating provision.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            // If the update fails, return the "UpdateProvision" page with the current model state
            return Page();
        }

        public async Task<IActionResult> OnGetDeleteProvisionAsync(int id) 
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/provision/{id}"); 
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<GetProvisionDto>>();
                if (result.Success)
                {
                    return RedirectToPage("DeleteProvision", new { id = result.Data.Id }); // Assuming you have a "DeleteProvision.cshtml" page
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                    return RedirectToPage("/Provisions/Index"); // Redirect to the index page or another suitable page
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToPage("/Provisions/Index"); // Redirect to the index page or another suitable page
            }
        }
    }
}
