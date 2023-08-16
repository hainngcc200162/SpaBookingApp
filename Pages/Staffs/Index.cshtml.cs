using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SpaBookingApp.Dtos.Staff;

namespace SpaBookingApp.Pages.Staffs
{
    public class StaffModel : PageModel
    {
        private readonly IStaffService _staffService;
        private readonly HttpClient _httpClient;

        public StaffModel(IStaffService staffService, IHttpClientFactory httpClientFactory)
        {
            _staffService = staffService;
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("http://localhost:5119/");
        }

        public List<GetStaffDto> Staffs { get; set; }
        public string ErrorMessage { get; set; }

        [BindProperty(SupportsGet = true)]
        public string searchByName { get; set; } // Property to bind to search input

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 0; // Property to bind to page index
        [BindProperty(SupportsGet = true)]
        public StaffGender? searchByGender { get; set; }

        public async Task OnGetAsync()
        {

            var response = await _staffService.GetAllStaffs(PageIndex, searchByName, searchByGender);

            if (response.Success)
            {
                Staffs = response.Data;
            }
            else
            {
                ErrorMessage = response.Message;
            }
        }
    }
}
