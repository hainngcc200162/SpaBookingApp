using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<ServiceResponse<List<GetStaffDto>>> AddStaff([FromForm] AddStaffDto newStaff)
        {
            var serviceResponse = new ServiceResponse<List<GetStaffDto>>();
            // Check if a staff member with the same email already exists
            var existingStaffWithEmail = await _context.Staffs.FirstOrDefaultAsync(s => s.Email == newStaff.Email);
            if (existingStaffWithEmail != null)
            {
                // If a staff member with the same email already exists, handle the error here or return an error message
                serviceResponse.Success = false;
                serviceResponse.Message = "Staff with the same email already exists.";
                return serviceResponse;
            }

            // Check if a staff member with the same identifier (e.g., name) already exists
            var existingStaff = await _context.Staffs.FirstOrDefaultAsync(s => s.Name == newStaff.Name);
            if (existingStaff != null)
            {
                // If the staff member already exists, you can handle the error here or return an error message
                serviceResponse.Success = false;
                serviceResponse.Message = "Staff already exists.";
                return serviceResponse;
            }
            var staff = _mapper.Map<Staff>(newStaff);

            if (newStaff.Poster != null)
            {
                var fileName = Path.GetFileName(newStaff.Poster.FileName);
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await newStaff.Poster.CopyToAsync(stream);
                }

                staff.PosterName = "/uploads/" + fileName;
            }

            _context.Staffs.Update(staff);
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

                staff.IsDeleted = true;
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

        public async Task<ServiceResponse<List<GetStaffDto>>> GetAllStaffs(int pageIndex, int pageSize, string searchByName, StaffGender? searchByGender)
        { // Số lượng nhân viên hiển thị trên mỗi trang

            var serviceResponse = new ServiceResponse<List<GetStaffDto>>();

            try
            {
                IQueryable<Staff> query = _context.Staffs;

                // Áp dụng tìm kiếm theo tên nếu có giá trị searchByName được cung cấp
                if (!string.IsNullOrEmpty(searchByName))
                {
                    query = query.Where(s => s.Name.Contains(searchByName));
                }

                // Áp dụng tìm kiếm theo giới tính nếu có giá trị searchByGender được cung cấp
                if (searchByGender.HasValue)
                {
                    query = query.Where(s => s.Gender == searchByGender.Value);
                }

                query = query.Where(d => !d.IsDeleted);

                var totalCount = await query.CountAsync();

                var pagedStaffs = await query
                    .OrderByDescending(s => s.Id)
                    .Skip(pageIndex * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var pageInfo = new PageInformation
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };

                serviceResponse.Success = true;
                serviceResponse.Data = pagedStaffs.Select(s => _mapper.Map<GetStaffDto>(s)).ToList();
                serviceResponse.PageInformation = pageInfo;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

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

        public async Task<ServiceResponse<GetStaffDto>> UpdateStaff([FromForm] UpdateStaffDto updatedStaff)
        {
            var serviceResponse = new ServiceResponse<GetStaffDto>();
            try
            {
                var staff = await _context.Staffs.FirstOrDefaultAsync(s => s.Id == updatedStaff.Id);
                if (staff is null)
                {
                    throw new Exception($"Staff  with ID '{updatedStaff.Id}' not found");
                }

                var existingStaffWithSameName = await _context.Staffs.FirstOrDefaultAsync(s =>
            s.Name == updatedStaff.Name && s.Id != updatedStaff.Id);

                if (existingStaffWithSameName != null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"Staff with the same name already exists";
                    return serviceResponse;
                }

                var existingStaffWithSameEmail = await _context.Staffs.FirstOrDefaultAsync(s =>
            s.Email == updatedStaff.Email && s.Id != updatedStaff.Id);

                if (existingStaffWithSameEmail != null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"Staff with the same Email already exists";
                    return serviceResponse;
                }

                _mapper.Map(updatedStaff, staff);

                if (updatedStaff.Poster != null)
                {
                    var fileName = Path.GetFileName(updatedStaff.Poster.FileName);
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await updatedStaff.Poster.CopyToAsync(stream);
                    }

                    staff.PosterName = "/uploads/" + fileName;
                }

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
