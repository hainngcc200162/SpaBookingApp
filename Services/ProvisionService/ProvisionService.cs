using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SpaBookingApp.Services.ProvisionService
{
    public class ProvisionService : IProvisionService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public ProvisionService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<ServiceResponse<List<GetProvisionDto>>> AddProvision([FromForm] AddProvisionDto newProvision)
        {
            var serviceResponse = new ServiceResponse<List<GetProvisionDto>>();

            // Check if a provision with the same identifier (e.g., name) already exists
            var existingProvision = await _context.Provisions.FirstOrDefaultAsync(p => p.Name == newProvision.Name);
            if (existingProvision != null)
            {
                // If the provision already exists, you can handle the error here or return an error message
                serviceResponse.Success = false;
                serviceResponse.Message = "Provision already exists.";
                return serviceResponse;
            }
            var Provision = _mapper.Map<Provision>(newProvision);

            if (newProvision.Poster != null)
            {
                var fileName = Path.GetFileName(newProvision.Poster.FileName);
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await newProvision.Poster.CopyToAsync(stream);
                }

                Provision.PosterName = "/uploads/" + fileName;
            }

            _context.Provisions.Add(Provision);
            await _context.SaveChangesAsync();

            serviceResponse.Data = await _context.Provisions
                .Select(p => _mapper.Map<GetProvisionDto>(p))
                .ToListAsync();

            return serviceResponse;
        }
        public async Task<ServiceResponse<List<GetProvisionDto>>> DeleteProvision(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetProvisionDto>>();
            try
            {
                var Provision = await _context.Provisions.FirstOrDefaultAsync(p => p.Id == id);
                if (Provision is null)
                {
                    throw new Exception($"Provision with ID '{id}' not found");
                }

                Provision.IsDeleted = true;
                await _context.SaveChangesAsync();

                serviceResponse.Data = await _context.Provisions
                    .Select(p => _mapper.Map<GetProvisionDto>(p))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetProvisionDto>>> GetAllProvisions()
        {
            var serviceResponse = new ServiceResponse<List<GetProvisionDto>>();

            try
            {
                var dbProvisions = await _context.Provisions
                    .Where(p => !p.IsDeleted) // Lọc các Provision có IsDeleted là false
                    .ToListAsync();

                serviceResponse.Data = _mapper.Map<List<GetProvisionDto>>(dbProvisions);
                serviceResponse.Success = true;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }


        public async Task<ServiceResponse<GetProvisionDto>> GetProvisionById(int id)
        {
            var serviceResponse = new ServiceResponse<GetProvisionDto>();
            var dbProvision = await _context.Provisions
                .FirstOrDefaultAsync(p => p.Id == id);

            if (dbProvision is null)
            {
                throw new Exception($"Provision with ID '{id}' not found");
            }

            var getProvisionDto = _mapper.Map<GetProvisionDto>(dbProvision);
            getProvisionDto.PosterName = dbProvision.PosterName;
            serviceResponse.Data = getProvisionDto;
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetProvisionDto>> UpdateProvision([FromForm] UpdateProvisionDto updatedProvision)
        {
            var serviceResponse = new ServiceResponse<GetProvisionDto>();
            try
            {
                var Provision = await _context.Provisions
                    .FirstOrDefaultAsync(p => p.Id == updatedProvision.Id);
                if (Provision is null)
                {
                    throw new Exception($"Provision with ID '{updatedProvision.Id}' not found");
                }

                // Kiểm tra xem tên mới đã tồn tại cho một dịch vụ khác chưa
                var existingProvisionWithSameName = await _context.Provisions.FirstOrDefaultAsync(p =>
                    p.Name == updatedProvision.Name && p.Id != updatedProvision.Id);

                if (existingProvisionWithSameName != null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"Provision with the same name already exists";
                    return serviceResponse;
                }

                _mapper.Map(updatedProvision, Provision);

                if (updatedProvision.Poster != null)
                {
                    var fileName = Path.GetFileName(updatedProvision.Poster.FileName);
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await updatedProvision.Poster.CopyToAsync(stream);
                    }

                    Provision.PosterName = "/uploads/" + fileName;
                }

                await _context.SaveChangesAsync();
                serviceResponse.Data = _mapper.Map<GetProvisionDto>(Provision);
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