using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Services.StaffService
{
    public interface IStaffService
    {
        Task<ServiceResponse<List<GetStaffDto>>> GetAllStaffMembers();
        Task<ServiceResponse<GetStaffDto>> GetStaffMemberById(int id);
        Task<ServiceResponse<List<GetStaffDto>>> AddStaffMember(AddStaffDto newStaff);
        Task<ServiceResponse<GetStaffDto>> UpdateStaffMember(UpdateStaffDto updatedStaff);
        Task<ServiceResponse<GetStaffDto>> DeleteStaffMember(int id);
    }
}
