using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpaBookingApp.Dtos.Department;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpaBookingApp.Pages.Departments
{
    public class UpdateDepartmentModel : PageModel
    {
        private readonly HttpClient _httpClient;

        [BindProperty]
        public UpdateDepartmentDto Department { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public UpdateDepartmentModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://fspa.azurewebsites.net/");
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Department/{id}");
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<GetDepartmentDto>>();
                if (result.Success)
                {
                    var getDepartmentDto = result.Data;
                    Department = new UpdateDepartmentDto
                    {
                        Id = getDepartmentDto.Id,
                        Name = getDepartmentDto.Name,
                        OpeningHours = getDepartmentDto.OpeningHours,
                        Description = getDepartmentDto.Description
                    };
                    return Page();
                }
                else
                {
                    ErrorMessage = result.Message;
                    return RedirectToPage("/Departments/Index");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return RedirectToPage("/Departments/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/Department/{Department.Id}", Department);
                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<GetDepartmentDto>>();

                if (response.IsSuccessStatusCode && result.Success)
                {
                    SuccessMessage = "Department updated successfully.";
                    return RedirectToPage("/Departments/Index");
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

            // Ensure the error message is not empty
            if (string.IsNullOrEmpty(ErrorMessage))
            {
                ErrorMessage = "An error occurred while processing the request.";
            }

            return Page();
        }
    }
}
