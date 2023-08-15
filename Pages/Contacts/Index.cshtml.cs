using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpaBookingApp.Dtos.Contact;
using SpaBookingApp.Services.ContactService;
using System.Threading.Tasks;

namespace SpaBookingApp.Pages.Contacts
{
    public class IndexContactModel : PageModel
    {
        private readonly IContactService _contactService;
        private readonly HttpClient _httpClient;

        public IndexContactModel(HttpClient httpClient, IContactService contactService)
        {
            _contactService = contactService;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5119/");
        }

        public List<GetContactDto> Contacts { get; set; }
        public string ErrorMessage { get; set; }

        public async Task OnGetAsync(int? pageIndex)
        {
            // Check if pageIndex is null, and if so, set it to 0
            if (!pageIndex.HasValue)
            {
                pageIndex = 0;
            }

            try
            {
                var response = await _httpClient.GetAsync($"api/Contact/GetAll?pageIndex={pageIndex}");
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<List<GetContactDto>>>();
                if (result.Success)
                {
                    Contacts = result.Data;
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
        }

        public async Task<IActionResult> OnGetShowContactDetails(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Contact/{id}");
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<GetContactDto>>();
                if (result.Success)
                {
                    TempData["Contact"] = result.Data; // Pass the contact data to the next request using TempData
                    return RedirectToPage("ContactDetails"); // Assuming you have a "ContactDetails.cshtml" page to display the contact details
                }
                else
                {
                    ErrorMessage = result.Message;
                    return RedirectToPage("Index"); // Redirect to the index page or another suitable page
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return RedirectToPage("Index"); // Handle the exception and redirect appropriately
            }
        }

        public async Task<IActionResult> OnGetDeleteContactAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Contact/{id}");
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<GetContactDto>>();
                if (result.Success)
                {
                    return RedirectToPage("DeleteContact", new { id = result.Data.Id });
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                    return RedirectToPage("/Contacts/Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToPage("/Contacts/Index"); // Handle the exception and redirect appropriately
            }
        }
    }
}
