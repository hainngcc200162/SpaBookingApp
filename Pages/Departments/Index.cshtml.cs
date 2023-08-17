using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SpaBookingApp.Pages.Departments
{
    public class IndexModel : PageModel
    {
        private readonly IDepartmentService _departmentService;
        private readonly HttpClient _httpClient;

        public IndexModel(HttpClient httpClient, IDepartmentService departmentService)
        {
            _departmentService = departmentService;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5119/");
        }

        public List<GetDepartmentDto> Departments { get; set; }
        public PageInformation PageInformation { get; set; }
        public string ErrorMessage { get; set; }

        [BindProperty(SupportsGet = true)]
        public string searchByName { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 0;

        public async Task OnGetAsync()
        {
            var serviceResponse = await _departmentService.GetAllDepartments(PageIndex, searchByName);
            if (serviceResponse.Success)
            {
                Departments = serviceResponse.Data;
                PageInformation = serviceResponse.PageInformation;
            }
            else
            {
                ErrorMessage = serviceResponse.Message;
            }
        }

        public async Task<IActionResult> OnGetShowDepartmentDetails(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Department/{id}");
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<GetDepartmentDto>>();
                if (result.Success)
                {
                    TempData["Department"] = result.Data; // Pass the department data to the next request using TempData
                    return RedirectToPage("DepartmentDetails"); // Assuming you have a "DepartmentDetails.cshtml" page to display the department details
                }
                else
                {
                    ErrorMessage = result.Message;
                    return RedirectToPage("Index"); // Redirect to the index page or another suitable page
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return RedirectToPage("Index"); // Handle the exception and redirect appropriately
            }
        }

        public async Task<IActionResult> OnPostUpdateDepartmentAsync(int id, UpdateDepartmentDto updateDepartmentDto)
        {
            updateDepartmentDto.Id = id; // Set the ID of the department to be updated
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/Department/{id}", updateDepartmentDto);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<List<GetDepartmentDto>>>();
                if (result.Success)
                {
                    return RedirectToPage("Index"); // Redirect to the index page or another suitable page
                }
                else
                {
                    ErrorMessage = result.Message;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            // If the update fails or an exception occurs, return the "Index" page with the current model state
            return Page();
        }

        public async Task<IActionResult> OnGetDeleteDepartmentAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Department/{id}");
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<GetDepartmentDto>>();
                if (result.Success)
                {
                    return RedirectToPage("DeleteDepartment", new { id = result.Data.Id });
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                    return RedirectToPage("/Departments/Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToPage("/Departments/Index"); // Handle the exception and redirect appropriately
            }
        }
    }
}
