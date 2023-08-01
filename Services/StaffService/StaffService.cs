using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SpaBookingApp.Data;

namespace SpaBookingApp.Services.StaffService
{
    public class StaffService : IStaffService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public StaffService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<ServiceResponse<List<GetStaffDto>>> AddStaffMember(AddStaffDto newStaff)
        {
            var serviceResponse = new ServiceResponse<List<GetStaffDto>>();
            var staffMember = _mapper.Map<Staff>(newStaff);

            _context.StaffMembers.Add(staffMember);
            await _context.SaveChangesAsync();
            serviceResponse.Data = await _context.StaffMembers
                .Select(s => _mapper.Map<GetStaffDto>(s))
                .ToListAsync();

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetStaffDto>> DeleteStaffMember(int id)
        {
            var serviceResponse = new ServiceResponse<GetStaffDto>();
            try
            {
                var staffMember = await _context.StaffMembers.FirstOrDefaultAsync(s => s.Id == id);
                if (staffMember is null)
                {
                    throw new Exception($"Staff member with ID '{id}' not found");
                }

                _context.StaffMembers.Remove(staffMember);
                await _context.SaveChangesAsync();

                serviceResponse.Data = _mapper.Map<GetStaffDto>(staffMember);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetStaffDto>>> GetAllStaffMembers()
        {
            var serviceResponse = new ServiceResponse<List<GetStaffDto>>();
            var dbStaffMembers = await _context.StaffMembers.ToListAsync();

            serviceResponse.Data = dbStaffMembers.Select(s => _mapper.Map<GetStaffDto>(s)).ToList();

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetStaffDto>> GetStaffMemberById(int id)
        {
            var serviceResponse = new ServiceResponse<GetStaffDto>();
            var dbStaffMember = await _context.StaffMembers.FirstOrDefaultAsync(s => s.Id == id);
            if (dbStaffMember is null)
            {
                throw new Exception($"Staff member with ID '{id}' not found");
            }
            serviceResponse.Data = _mapper.Map<GetStaffDto>(dbStaffMember);
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetStaffDto>> UpdateStaffMember(UpdateStaffDto updatedStaff)
        {
            var serviceResponse = new ServiceResponse<GetStaffDto>();
            try
            {
                var staffMember = await _context.StaffMembers.FirstOrDefaultAsync(s => s.Id == updatedStaff.Id);
                if (staffMember is null)
                {
                    throw new Exception($"Staff member with ID '{updatedStaff.Id}' not found");
                }

                _mapper.Map(updatedStaff, staffMember);
                await _context.SaveChangesAsync();
                serviceResponse.Data = _mapper.Map<GetStaffDto>(staffMember);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }
    }
}
