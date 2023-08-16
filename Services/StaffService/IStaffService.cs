using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SpaBookingApp.Services.StaffService
{
    public interface IStaffService
    {
        Task<ServiceResponse<List<GetStaffDto>>> GetAllStaffs(int pageIndex, string searchByName,StaffGender? searchByGender);
        Task<ServiceResponse<GetStaffDto>> GetStaffById(int id);
        Task<ServiceResponse<List<GetStaffDto>>> AddStaff([FromForm] AddStaffDto newStaff);
        Task<ServiceResponse<GetStaffDto>> UpdateStaff([FromForm] UpdateStaffDto updatedStaff);
        Task<ServiceResponse<GetStaffDto>> DeleteStaff(int id);
    }
}
