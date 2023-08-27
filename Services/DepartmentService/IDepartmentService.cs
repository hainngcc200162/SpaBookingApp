using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Services.DepartmentService
{
    public interface IDepartmentService
    {
        Task<ServiceResponse<List<GetDepartmentDto>>> GetAllDepartments(int pageIndex, int pageSize, string searchByName);
        Task<ServiceResponse<GetDepartmentDto>> GetDepartmentById(int id);
        Task<ServiceResponse<List<GetDepartmentDto>>> AddDepartment(AddDepartmentDto newDepartment);
        Task<ServiceResponse<GetDepartmentDto>> UpdateDepartment(UpdateDepartmentDto updatedDepartment);
        Task<ServiceResponse<GetDepartmentDto>> DeleteDepartment(int id);
    }
}