using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpaBookingApp.Dtos.Staff;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SpaBookingApp.Pages.Staffs
{
    public class CreateStaffModel : PageModel
    {
        private readonly HttpClient _httpClient;

        [BindProperty]
        public AddStaffDto Staff { get; set; }

        public List<GetStaffDto> StaffList { get; set; }

        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        public CreateStaffModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5119/");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(Staff.Name), "Name");
                content.Add(new StringContent(Staff.Gender.ToString()), "Gender");
                content.Add(new StringContent(Staff.Email), "Email");
                content.Add(new StringContent(Staff.Description), "Description");
                content.Add(new StreamContent(Staff.Poster.OpenReadStream()), "Poster", Staff.Poster.FileName);

                var response = await _httpClient.PostAsync("api/Staff", content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<List<GetStaffDto>>>();
                if (result.Success)
                {
                    SuccessMessage = "Staff created successfully.";
                    return RedirectToPage("/Staff/Index");
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

            if (string.IsNullOrEmpty(ErrorMessage))
            {
                ErrorMessage = "An error occurred while processing the request.";
            }

            return Page();
        }
    }
}
