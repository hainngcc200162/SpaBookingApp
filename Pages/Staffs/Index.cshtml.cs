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

        public async Task OnGetAsync()
        {

            var response = await _staffService.GetAllStaffs();

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
