using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace SpaBookingApp.Pages.Home
{
    public class ProvisionModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IProvisionService _provisionService; // Sử dụng IProvisionService thay vì IProductService
        private readonly IDepartmentService _departmentService;

        public ProvisionModel(HttpClient httpClient, IProvisionService provisionService, IDepartmentService departmentService)
        {
            _httpClient = httpClient;
            _provisionService = provisionService;
            _departmentService = departmentService;
            _httpClient.BaseAddress = new Uri("http://localhost:5119/"); // Thay thế bằng URL cơ sở của API của bạn
        }

        public List<GetProvisionDto> Provisions { get; set; }
        public List<GetDepartmentDto> Departments { get; set; }
        public string ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            // Sử dụng Task.WhenAll để gọi đồng thời cả hai tác vụ
            var provisionTask = LoadProvisionsAsync();
            var departmentTask = LoadDepartmentsAsync();

            // Đợi cho cả hai tác vụ hoàn thành
            await Task.WhenAll(provisionTask, departmentTask);

            // Kiểm tra kết quả của tác vụ và xử lý tương ứng
            if (provisionTask.Result && departmentTask.Result)
            {
                // Cả hai tác vụ đã hoàn thành thành công
            }
            else
            {
                // Một hoặc cả hai tác vụ không thành công
            }
        }

        private async Task<bool> LoadProvisionsAsync()
        {
            var response = await _httpClient.GetAsync("api/provision/GetAll");
            if (response.IsSuccessStatusCode)
            {
                var serviceResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<List<GetProvisionDto>>>();
                if (serviceResponse != null && serviceResponse.Success)
                {
                    Provisions = serviceResponse.Data;
                    return true;
                }
                else
                {
                    ErrorMessage = serviceResponse?.Message ?? "Error";
                    return false;
                }
            }
            else
            {
                ErrorMessage = "Can not connect to API.!";
                return false;
            }
        }

        private async Task<bool> LoadDepartmentsAsync()
        {
            int pageIndex = 0; // Provide the appropriate pageIndex value
            int pageSize = 100; // Provide the appropriate pageSize value
            string searchByName = "";

            var departmentResponse = await _departmentService.GetAllDepartments(pageIndex, pageSize, searchByName);
            if (departmentResponse.Success)
            {
                Departments = departmentResponse.Data;
                return true;
            }
            else
            {
                ErrorMessage = departmentResponse.Message;
                return false;
            }
        }

    }
}