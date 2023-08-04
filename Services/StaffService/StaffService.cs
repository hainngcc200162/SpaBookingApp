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

        public async Task<ServiceResponse<List<GetStaffDto>>> AddStaff(AddStaffDto newStaff)
        {
            var serviceResponse = new ServiceResponse<List<GetStaffDto>>();
            var staff = _mapper.Map<Staff>(newStaff);

            _context.Staffs.Add(staff);
            await _context.SaveChangesAsync();
            serviceResponse.Data = await _context.Staffs
                .Select(s => _mapper.Map<GetStaffDto>(s))
                .ToListAsync();

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetStaffDto>> DeleteStaff(int id)
        {
            var serviceResponse = new ServiceResponse<GetStaffDto>();
            try
            {
                var staff = await _context.Staffs.FirstOrDefaultAsync(s => s.Id == id);
                if (staff is null)
                {
                    throw new Exception($"Staff  with ID '{id}' not found");
                }

                _context.Staffs.Remove(staff);
                await _context.SaveChangesAsync();

                serviceResponse.Data = _mapper.Map<GetStaffDto>(staff);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetStaffDto>>> GetAllStaffs()
        {
            var serviceResponse = new ServiceResponse<List<GetStaffDto>>();
            var dbStaffs = await _context.Staffs.ToListAsync();

            serviceResponse.Data = dbStaffs.Select(s => _mapper.Map<GetStaffDto>(s)).ToList();

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetStaffDto>> GetStaffById(int id)
        {
            var serviceResponse = new ServiceResponse<GetStaffDto>();
            var dbStaff = await _context.Staffs.FirstOrDefaultAsync(s => s.Id == id);
            if (dbStaff is null)
            {
                throw new Exception($"Staff  with ID '{id}' not found");
            }
            serviceResponse.Data = _mapper.Map<GetStaffDto>(dbStaff);
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetStaffDto>> UpdateStaff(UpdateStaffDto updatedStaff)
        {
            var serviceResponse = new ServiceResponse<GetStaffDto>();
            try
            {
                var staff = await _context.Staffs.FirstOrDefaultAsync(s => s.Id == updatedStaff.Id);
                if (staff is null)
                {
                    throw new Exception($"Staff  with ID '{updatedStaff.Id}' not found");
                }

                _mapper.Map(updatedStaff, staff);
                await _context.SaveChangesAsync();
                serviceResponse.Data = _mapper.Map<GetStaffDto>(staff);
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
