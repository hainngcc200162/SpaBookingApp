using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SpaBookingApp.Dtos.Provision;
using SpaBookingApp.Dtos.Category;
using SpaBookingApp.Services.ProvisionService;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace SpaBookingApp.Pages.Provisions
{
    public class UpdateProvisionModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IProvisionService _provisionService;

        [BindProperty]
        public UpdateProvisionDto Provision { get; set; }

        public List<SelectListItem> Categories { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public UpdateProvisionModel(HttpClient httpClient, IProvisionService provisionService)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5119/");
            _provisionService = provisionService;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Provision/{id}");
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<GetProvisionDto>>();
                if (result.Success)
                {
                    var getProvisionDto = result.Data;
                    Provision = new UpdateProvisionDto
                    {
                        Id = getProvisionDto.Id,
                        Name = getProvisionDto.Name,
                        Description = getProvisionDto.Description,
                        Price = getProvisionDto.Price,
                        DurationMinutes = getProvisionDto.DurationMinutes,
                        NumberOfExecutions = getProvisionDto.NumberOfExecutions,
                        Status = getProvisionDto.Status,
                        PosterName = getProvisionDto.PosterName
                    };

                    return Page();
                }
                else
                {
                    ErrorMessage = result.Message;
                    return RedirectToPage("/Products/Index");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return RedirectToPage("/Provisions/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (Provision.Poster == null)
                {
                    var getProvisionResponse = await _provisionService.GetProvisionById(Provision.Id);
                    if (getProvisionResponse.Data != null && getProvisionResponse.Success)
                    {
                        Provision.PosterName = getProvisionResponse.Data.PosterName;
                    }
                }

                var response = await _httpClient.PutAsJsonAsync($"api/Provision/{Provision.Id}", Provision);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<List<GetProvisionDto>>>();
                if (result.Success)
                {
                    return RedirectToPage("/Provisions/Index");
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
