using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
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
            _contactService = contactService ?? throw new ArgumentNullException(nameof(contactService));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpClient.BaseAddress = new Uri("http://localhost:5119/");

            // Khởi tạo Subjects
            Subjects = new List<GetSubjectDto>();
        }

        public List<GetSubjectDto> Subjects { get; set; }

        public async Task OnGetAsync()
        {
            await LoadSubjects();
        }

        public async Task<IActionResult> OnPostAsync(AddContactDto newContact)
        {
            if (!ModelState.IsValid)
            {
                await LoadSubjects();
                return Page();
            }

            // Kiểm tra định dạng số điện thoại
            if (!IsValidPhoneNumber(newContact.Phone))
            {
                await LoadSubjects();
                ViewData["ErrorMessage"] = "Invalid phone number format.";
                return Page();
            }

            // Kiểm tra định dạng email
            if (!IsValidEmail(newContact.Email))
            {
                await LoadSubjects();
                ViewData["ErrorMessage"] = "Invalid email address format.";
                return Page();
            }

            // Xử lý dữ liệu khi hợp lệ
            var response = await _contactService.AddContact(newContact);

            if (!response.Success)
            {
                await LoadSubjects();
                ViewData["ErrorMessage"] = response.Message;
                return Page();
            }

            // Redirect hoặc hiển thị thông báo thành công
            await LoadSubjects();
            ViewData["SuccessMessage"] = "Contact added successfully!";
            return Page();
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            Regex regex = new Regex(@"^[0-9]{10}$");
            return regex.IsMatch(phoneNumber);
        }

        private bool IsValidEmail(string email)
        {
            // Biểu thức chính quy cho địa chỉ email
            Regex regex = new Regex(@"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$");
            return regex.IsMatch(email);
        }

        private async Task LoadSubjects()
        {
            var subjectResponse = await _httpClient.GetFromJsonAsync<ServiceResponse<List<GetSubjectDto>>>("api/Subject/GetAll");
            Subjects = subjectResponse?.Data ?? new List<GetSubjectDto>();
        }
    }
}
