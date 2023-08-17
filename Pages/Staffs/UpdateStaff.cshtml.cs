using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SpaBookingApp.Dtos.Staff;
using SpaBookingApp.Services.StaffService;
using Microsoft.AspNetCore.Authorization;

namespace SpaBookingApp.Pages.Staffs
{
    public class UpdateStaffModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IStaffService _staffService;

        [BindProperty]
        public UpdateStaffDto Staff { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public UpdateStaffModel(HttpClient httpClient, IStaffService staffService)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5119/");
            _staffService = staffService;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Staff/{id}");
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<GetStaffDto>>();
                if (result.Success)
                {
                    var getStaffDto = result.Data;
                    Staff = new UpdateStaffDto
                    {
                        Id = getStaffDto.Id,
                        Name = getStaffDto.Name,
                        Gender = getStaffDto.Gender,
                        Email = getStaffDto.Email,
                        Description = getStaffDto.Description,
                        PosterName = getStaffDto.PosterName
                    };

                    return Page();
                }
                else
                {
                    ErrorMessage = result.Message;
                    return RedirectToPage("/Staffs/Index");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return RedirectToPage("/Staffs/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (Staff.Poster == null)
                {
                    var getStaffResponse = await _staffService.GetStaffById(Staff.Id);
                    if (getStaffResponse.Data != null && getStaffResponse.Success)
                    {
                        Staff.PosterName = getStaffResponse.Data.PosterName;
                    }
                }

                var response = await _httpClient.PutAsJsonAsync($"api/Staff/{Staff.Id}", Staff);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<List<GetStaffDto>>>();
                if (result.Success)
                {
                    return RedirectToPage("/Staffs/Index");
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
