using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpaBookingApp.Dtos.Staff;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpaBookingApp.Pages.Staffs
{
    public class DeleteStaffModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DeleteStaffModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://fspa.azurewebsites.net/"); // Thay thế bằng URL cơ sở của API delete staff
        }

        [BindProperty]
        public GetStaffDto Staff { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/Staff/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<ServiceResponse<GetStaffDto>>();
                    Staff = data.Data;
                    return Page();
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

            return RedirectToPage("Index");
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/api/Staff/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("Index");
                }
                else
                {
                    ErrorMessage = "Error deleting staff.";
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
