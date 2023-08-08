using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SpaBookingApp.Data;

namespace SpaBookingApp.Services.DepartmentService
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public DepartmentService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<ServiceResponse<List<GetDepartmentDto>>> AddDepartment(AddDepartmentDto newDepartment)
        {
            var serviceResponse = new ServiceResponse<List<GetDepartmentDto>>();
            var Department = _mapper.Map<Department>(newDepartment);

            _context.Departments.Add(Department);
            await _context.SaveChangesAsync();
            serviceResponse.Data = await _context.Departments
                .Select(a => _mapper.Map<GetDepartmentDto>(a))
                .ToListAsync();

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetDepartmentDto>> DeleteDepartment(int id)
        {
            var serviceResponse = new ServiceResponse<GetDepartmentDto>();
            try
            {
                var Department = await _context.Departments.FirstOrDefaultAsync(a => a.Id == id);
                if (Department is null)
                {
                    throw new Exception($"Department with ID '{id}' not found");
                }

                _context.Departments.Remove(Department);
                await _context.SaveChangesAsync();

                serviceResponse.Data = _mapper.Map<GetDepartmentDto>(Department);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetDepartmentDto>>> GetAllDepartments()
        {
            var serviceResponse = new ServiceResponse<List<GetDepartmentDto>>();
            var dbDepartments = await _context.Departments.ToListAsync();

            serviceResponse.Data = dbDepartments.Select(a => _mapper.Map<GetDepartmentDto>(a)).ToList();

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetDepartmentDto>> GetDepartmentById(int id)
        {
            var serviceResponse = new ServiceResponse<GetDepartmentDto>();
            var dbDepartment = await _context.Departments.FirstOrDefaultAsync(a => a.Id == id);
            if (dbDepartment is null)
            {
                throw new Exception($"Department with ID '{id}' not found");
            }
            serviceResponse.Data = _mapper.Map<GetDepartmentDto>(dbDepartment);
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetDepartmentDto>> UpdateDepartment(UpdateDepartmentDto updatedDepartment)
        {
            var serviceResponse = new ServiceResponse<GetDepartmentDto>();
            try
            {
                var Department = await _context.Departments.FirstOrDefaultAsync(a => a.Id == updatedDepartment.Id);
                if (Department is null)
                {
                    throw new Exception($"Department with ID '{updatedDepartment.Id}' not found");
                }

                _mapper.Map(updatedDepartment, Department);
                await _context.SaveChangesAsync();
                serviceResponse.Data = _mapper.Map<GetDepartmentDto>(Department);
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
