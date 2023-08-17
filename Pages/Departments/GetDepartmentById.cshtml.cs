using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpaBookingApp.Dtos.Department;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SpaBookingApp.Pages.Departments
{
    public class GetDepartmentByIdModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public GetDepartmentDto Department { get; set; }

        public string ErrorMessage { get; set; }

        public GetDepartmentByIdModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5119/");
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<GetDepartmentDto>>($"api/Department/{id}");
            Department = response?.Data;

            if (Department == null)
            {
                ErrorMessage = response?.Message ?? "Department not found";
                return Page();
            }

            return Page();
        }
    }
}
