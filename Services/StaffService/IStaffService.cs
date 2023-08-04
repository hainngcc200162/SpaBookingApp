using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Services.StaffService
{
    public interface IStaffService
    {
        Task<ServiceResponse<List<GetStaffDto>>> GetAllStaffs();
        Task<ServiceResponse<GetStaffDto>> GetStaffById(int id);
        Task<ServiceResponse<List<GetStaffDto>>> AddStaff(AddStaffDto newStaff);
        Task<ServiceResponse<GetStaffDto>> UpdateStaff(UpdateStaffDto updatedStaff);
        Task<ServiceResponse<GetStaffDto>> DeleteStaff(int id);
    }
}
