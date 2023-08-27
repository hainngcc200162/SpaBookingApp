using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SpaBookingApp.Dtos.Staff;

namespace SpaBookingApp.Pages.Staffs
{
    public class StaffModel : PageModel
    {
        private readonly IStaffService _staffService;
        private readonly HttpClient _httpClient;

        public StaffModel(IStaffService staffService, IHttpClientFactory httpClientFactory)
        {
            _staffService = staffService;
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("http://localhost:5119/");
        }

        public List<GetStaffDto> Staffs { get; set; }
        public PageInformation PageInformation { get; set; }
        public string ErrorMessage { get; set; }

        [BindProperty(SupportsGet = true)]
        public string searchByName { get; set; } // Property to bind to search input

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 0; // Property to bind to page index
        [BindProperty(SupportsGet = true)]
        public StaffGender? searchByGender { get; set; }

        public async Task OnGetAsync()
        {
            int pageSize =4;

            var response = await _staffService.GetAllStaffs(PageIndex,pageSize, searchByName, searchByGender);

            if (response.Success)
            {
                Staffs = response.Data;
                PageInformation = response.PageInformation;
            }
            else
            {
                ErrorMessage = response.Message;
            }
        }

        public async Task<IActionResult> OnGetShowStaffDetails(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/staff/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<ServiceResponse<GetStaffDto>>();
                    var staff = data.Data;
                    TempData["Staff"] = staff;
                    return RedirectToPage("StaffDetails");
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

            return RedirectToPage("Index"); // Redirect to the index page or another suitable page
        }

        public async Task<IActionResult> OnPostUpdateAsync(int id, UpdateStaffDto updateStaffDto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/staff/{id}", updateStaffDto);
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("/");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error updating staff.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return Page();
        }

        public async Task<IActionResult> OnGetDeleteStaffAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/staff/{id}");
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<GetStaffDto>>();
                if (result.Success)
                {
                    return RedirectToPage("DeleteStaff", new { id = result.Data.Id });
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                    return RedirectToPage("/Staffs/Index"); // Redirect to the index page or another suitable page
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToPage("/Staffs/Index"); // Redirect to the index page or another suitable page
            }
        }
    }
}
