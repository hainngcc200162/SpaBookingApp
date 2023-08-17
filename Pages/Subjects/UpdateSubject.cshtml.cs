using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpaBookingApp.Dtos.Subject;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SpaBookingApp.Pages.Subjects
{
    public class UpdateSubjectModel : PageModel
    {
        private readonly HttpClient _httpClient;

        [BindProperty]
        public UpdateSubjectDto Subject { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public UpdateSubjectModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5119/");
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Subject/{id}");
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<GetSubjectDto>>();
                if (result.Success)
                {
                    Subject = new UpdateSubjectDto
                    {
                        Id = result.Data.Id,
                        Name = result.Data.Name
                    };
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
                return RedirectToPage("/Subjects/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/Subject/{Subject.Id}", Subject);
                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<GetSubjectDto>>();

                if (response.IsSuccessStatusCode && result.Success)
                {
                    SuccessMessage = "Subject updated successfully.";
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

            // Ensure the error message is not empty
            if (string.IsNullOrEmpty(ErrorMessage))
            {
                ErrorMessage = "An error occurred while processing the request.";
            }

            return Page();
        }
    }
}
