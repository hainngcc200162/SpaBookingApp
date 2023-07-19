using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpaBookingApp.Dtos.Provision;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SpaBookingApp.Pages.Provisions
{
    public class CreateProvisionModel : PageModel
    {
        private readonly HttpClient _httpClient;

        [BindProperty]
        public AddProvisionDto Provision { get; set; }

        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        public CreateProvisionModel(HttpClient httpClient)
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
                content.Add(new StringContent(Provision.Name), "Name");
                content.Add(new StringContent(Provision.Price.ToString()), "Price");
                content.Add(new StringContent(Provision.Description), "Description");
                content.Add(new StringContent(Provision.Status.ToString()), "Status");
                content.Add(new StreamContent(Provision.Poster.OpenReadStream()), "Poster", Provision.Poster.FileName);

                var response = await _httpClient.PostAsync("api/Provision", content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<List<GetProvisionDto>>>();
                // Console.WriteLine(result);
                if (result.Success)
                {
                    SuccessMessage = "Product created successfully.";
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

            return Page();
        }

    }
}
