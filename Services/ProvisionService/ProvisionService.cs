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
            var Provision = _mapper.Map<Provision>(newProvision);

            if (newProvision.Poster != null)
            {
                var fileName = Path.GetFileName(newProvision.Poster.FileName);
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await newProvision.Poster.CopyToAsync(stream);
                }

                Provision.PosterName = "/images/" + fileName;
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

                _context.Provisions.Remove(Provision);
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
            var dbProvisions = await _context.Provisions
                .ToListAsync();

            serviceResponse.Data = _mapper.Map<List<GetProvisionDto>>(dbProvisions);

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

                _mapper.Map(updatedProvision, Provision);

                if (updatedProvision.Poster != null)
                {
                    var fileName = Path.GetFileName(updatedProvision.Poster.FileName);
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await updatedProvision.Poster.CopyToAsync(stream);
                    }

                    Provision.PosterName = "/images/" + fileName;
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