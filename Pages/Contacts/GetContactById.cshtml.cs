using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpaBookingApp.Dtos.Contact;

namespace SpaBookingApp.Pages.Contacts
{
    public class GetContactByIdModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public GetContactDto Contact { get; set; }
        public string ErrorMessage { get; set; }

        public GetContactByIdModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5119/"); // Thay thế bằng URL cơ sở của API của bạn
        }

        public async Task<IActionResult> OnGet(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/Contact/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<ServiceResponse<GetContactDto>>();
                    Contact = data.Data;
                }
                else
                {
                    ErrorMessage = "Error retrieving Contact details.";
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
