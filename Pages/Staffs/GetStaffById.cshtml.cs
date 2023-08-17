using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SpaBookingApp.Dtos.Staff;

namespace SpaBookingApp.Pages.Staffs
{
    public class GetStaffByIdModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public GetStaffDto Staff { get; set; }
        public string ErrorMessage { get; set; }

        public GetStaffByIdModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5119/"); // Thay thế bằng URL cơ sở của API của bạn
        }

        public async Task<IActionResult> OnGet(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/Staff/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<ServiceResponse<GetStaffDto>>();
                    Staff = data.Data;
                }
                else
                {
                    ErrorMessage = "Error retrieving staff details.";
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
