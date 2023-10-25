using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpaBookingApp.Dtos.Subject;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpaBookingApp.Pages.Subjects
{
    public class DeleteSubjectModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DeleteSubjectModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://fspa.azurewebsites.net/"); // Update the base address as needed
        }

        [BindProperty]
        public GetSubjectDto Subject { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Subject/{id}");
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<GetSubjectDto>>();
                if (result.Success)
                {
                    Subject = result.Data;
                    return Page();
                }
                else
                {
                    ErrorMessage = result.Message;
                    return RedirectToPage("/Subjects/Index");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return RedirectToPage("/Subjects/Index"); // Handle the exception and redirect appropriately
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Subject/{id}");
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<List<GetSubjectDto>>>();
                if (result.Success)
                {
                    return RedirectToPage("/Subjects/Index");
                }
                else
                {
                    ErrorMessage = result.Message;
                    return RedirectToPage("/Subjects/Index");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return RedirectToPage("/Subjects/Index");
            }
        }
    }
}
