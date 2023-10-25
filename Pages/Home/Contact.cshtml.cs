using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpaBookingApp.Services.ContactService;

namespace SpaBookingApp.Pages.Home
{
    public class ContactModel : PageModel
    {
        private readonly IContactService _contactService;
        private readonly HttpClient _httpClient;

        public ContactModel(IContactService contactService, HttpClient httpClient)
        {
            _contactService = contactService;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://fspa.azurewebsites.net/");
        }

        public List<GetSubjectDto> Subjects { get; set; } // Đảm bảo có định nghĩa kiểu dữ liệu chính xác ở đây

        public async Task OnGetAsync()
        {
            await LoadSubjects();
        }

        public async Task<IActionResult> OnPostAsync(AddContactDto newContact)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var response = await _contactService.AddContact(newContact);

            if (!response.Success)
            {
                ModelState.AddModelError("", response.Message);
                return Page();
            }

            // Redirect to a success page or display a success message
            // For example: return RedirectToPage("/Home/ContactSuccess");
            await LoadSubjects();
            // Alternatively, return a View with a success message
            ViewData["SuccessMessage"] = "Contact added successfully!";
            return Page();
        }

        private async Task LoadSubjects()
        {
            var subjectResponse = await _httpClient.GetFromJsonAsync<ServiceResponse<List<GetSubjectDto>>>("api/Subject/GetAll");
            Subjects = subjectResponse?.Data;
        }
    }
}
