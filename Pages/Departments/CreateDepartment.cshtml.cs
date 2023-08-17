using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpaBookingApp.Dtos.Department;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SpaBookingApp.Pages.Departments
{
    public class CreateDepartmentModel : PageModel
    {
        private readonly HttpClient _httpClient;

        [BindProperty]
        public AddDepartmentDto Department { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public CreateDepartmentModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5119/");
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Department", Department);
                response.EnsureSuccessStatusCode(); // Throw an exception if the HTTP response is not successful

                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<List<GetDepartmentDto>>>();
                if (result.Success)
                {
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

            return Page();
        }
    }
}
