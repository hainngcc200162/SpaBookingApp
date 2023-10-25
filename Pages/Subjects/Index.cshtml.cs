using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpaBookingApp.Dtos.Subject;
using SpaBookingApp.Services; // Make sure to include the appropriate namespace

namespace SpaBookingApp.Pages.Subjects
{
    public class IndexModel : PageModel
    {
        private readonly ISubjectService _subjectService; // Update with the appropriate service interface
        private readonly HttpClient _httpClient;

        public IndexModel(HttpClient httpClient, ISubjectService subjectService) // Update with the appropriate service interface
        {
            _subjectService = subjectService;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://fspa.azurewebsites.net/");
        }

        public List<GetSubjectDto> Subjects { get; set; }
        public PageInformation PageInformation { get; set; }
        public string ErrorMessage { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 0;

        public async Task OnGetAsync()
        {
            var serviceResponse = await _subjectService.GetAllSubjects(PageIndex);
            if (serviceResponse.Success)
            {
                Subjects = serviceResponse.Data;
                PageInformation = serviceResponse.PageInformation;
            }
            else
            {
                ErrorMessage = serviceResponse.Message;
            }
        }

        public async Task<IActionResult> OnGetShowSubjectDetails(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Subject/{id}");
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<GetSubjectDto>>();
                if (result.Success)
                {
                    TempData["Subject"] = result.Data;
                    return RedirectToPage("SubjectDetails");
                }
                else
                {
                    ErrorMessage = result.Message;
                    return RedirectToPage("Index");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return RedirectToPage("Index");
            }
        }

        public async Task<IActionResult> OnPostUpdateSubjectAsync(int id, UpdateSubjectDto updateSubjectDto)
        {
            updateSubjectDto.Id = id;
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/Subject/{id}", updateSubjectDto);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<List<GetSubjectDto>>>();
                if (result.Success)
                {
                    return RedirectToPage("Index");
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

        public async Task<IActionResult> OnGetDeleteSubjectAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Subject/{id}");
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<GetSubjectDto>>();
                if (result.Success)
                {
                    return RedirectToPage("DeleteSubject", new { id = result.Data.Id });
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                    return RedirectToPage("/Subjects/Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToPage("/Subjects/Index");
            }
        }
    }
}
