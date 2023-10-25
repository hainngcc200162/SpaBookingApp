using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpaBookingApp.Dtos.Subject;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SpaBookingApp.Pages.Subjects
{
    public class CreateSubjectModel : PageModel
    {
        private readonly HttpClient _httpClient;

        [BindProperty]
        public AddSubjectDto Subject { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public CreateSubjectModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://fspa.azurewebsites.net/");
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
                var response = await _httpClient.PostAsJsonAsync("api/Subject", Subject);
                response.EnsureSuccessStatusCode(); // Throw an exception if the HTTP response is not successful

                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<List<GetSubjectDto>>>();
                if (result.Success)
                {
                    return RedirectToPage("/Subjects/Index");
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
