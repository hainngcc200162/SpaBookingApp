using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SpaBookingApp.Dtos.Provision; // Sử dụng mô hình Provision thay vì Product
using SpaBookingApp.Services.ProvisionService; // Sử dụng dịch vụ ProvisionService thay vì ProductService
using Microsoft.AspNetCore.Mvc;

namespace SpaBookingApp.Pages.Provisions // Đảm bảo namespace đúng với vị trí file
{
    public class ProvisionModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IProvisionService _provisionService; // Sử dụng IProvisionService thay vì IProductService

        public ProvisionModel(HttpClient httpClient, IProvisionService provisionService)
        {
            _httpClient = httpClient;
            _provisionService = provisionService;
            _httpClient.BaseAddress = new Uri("http://localhost:5119/"); // Thay thế bằng URL cơ sở của API của bạn
        }

        public List<GetProvisionDto> Provisions { get; set; } // Sử dụng GetProvisionDto thay vì GetProductDto
        public string ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            var response = await _httpClient.GetAsync("api/provision/GetAll"); // Sử dụng API endpoint cho Provisions
            if (response.IsSuccessStatusCode)
            {
                var serviceResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<List<GetProvisionDto>>>();
                if (serviceResponse != null && serviceResponse.Success)
                {
                    Provisions = serviceResponse.Data; // Sử dụng Provisions thay vì Products
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

        public async Task<IActionResult> OnGetShowProvisionDetails(int id) // Sử dụng OnGetShowProvisionDetails thay vì OnGetShowProductDetails
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/provision/{id}"); // Sử dụng API endpoint cho Provisions
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<ServiceResponse<GetProvisionDto>>();
                    var provision = data.Data; // Sử dụng provision thay vì product
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

        public async Task<IActionResult> OnPostUpdateAsync(int id, UpdateProvisionDto updateProvisionDto) // Sử dụng UpdateProvisionDto thay vì UpdateProductDto
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/provision/{id}", updateProvisionDto); // Sử dụng API endpoint cho Provisions
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("/"); // Redirect to the index page or another suitable page
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error updating provision."); // Sử dụng "provision" thay vì "product"
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            // If the update fails, return the "UpdateProvision" page with the current model state
            return Page();
        }

        public async Task<IActionResult> OnGetDeleteProvisionAsync(int id) // Sử dụng OnGetDeleteProvisionAsync thay vì OnGetDeleteProductAsync
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/provision/{id}"); // Sử dụng API endpoint cho Provisions
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
