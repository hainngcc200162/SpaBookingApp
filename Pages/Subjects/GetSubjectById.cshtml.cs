using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpaBookingApp.Dtos.Subject;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SpaBookingApp.Pages.Subjects
{
    public class GetSubjectByIdModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public GetSubjectDto Subject { get; set; }

        public string ErrorMessage { get; set; }

        public GetSubjectByIdModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5119/");
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<GetSubjectDto>>($"api/Subject/{id}");
            Subject = response?.Data;

            if (Subject == null)
            {
                ErrorMessage = response?.Message ?? "Subject not found";
                return Page();
            }

            return Page();
        }
    }
}
