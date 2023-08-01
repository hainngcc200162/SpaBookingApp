using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SpaBookingApp.Data;

namespace SpaBookingApp.Services.AppartmentService
{
    public class AppartmentService : IAppartmentService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public AppartmentService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<ServiceResponse<List<GetAppartmentDto>>> AddAppartment(AddAppartmentDto newAppartment)
        {
            var serviceResponse = new ServiceResponse<List<GetAppartmentDto>>();
            var appartment = _mapper.Map<Appartment>(newAppartment);

            _context.Appartments.Add(appartment);
            await _context.SaveChangesAsync();
            serviceResponse.Data = await _context.Appartments
                .Select(a => _mapper.Map<GetAppartmentDto>(a))
                .ToListAsync();

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetAppartmentDto>> DeleteAppartment(int id)
        {
            var serviceResponse = new ServiceResponse<GetAppartmentDto>();
            try
            {
                var appartment = await _context.Appartments.FirstOrDefaultAsync(a => a.Id == id);
                if (appartment is null)
                {
                    throw new Exception($"Appartment with ID '{id}' not found");
                }

                _context.Appartments.Remove(appartment);
                await _context.SaveChangesAsync();

                serviceResponse.Data = _mapper.Map<GetAppartmentDto>(appartment);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetAppartmentDto>>> GetAllAppartments()
        {
            var serviceResponse = new ServiceResponse<List<GetAppartmentDto>>();
            var dbAppartments = await _context.Appartments.ToListAsync();

            serviceResponse.Data = dbAppartments.Select(a => _mapper.Map<GetAppartmentDto>(a)).ToList();

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetAppartmentDto>> GetAppartmentById(int id)
        {
            var serviceResponse = new ServiceResponse<GetAppartmentDto>();
            var dbAppartment = await _context.Appartments.FirstOrDefaultAsync(a => a.Id == id);
            if (dbAppartment is null)
            {
                throw new Exception($"Appartment with ID '{id}' not found");
            }
            serviceResponse.Data = _mapper.Map<GetAppartmentDto>(dbAppartment);
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetAppartmentDto>> UpdateAppartment(UpdateAppartmentDto updatedAppartment)
        {
            var serviceResponse = new ServiceResponse<GetAppartmentDto>();
            try
            {
                var appartment = await _context.Appartments.FirstOrDefaultAsync(a => a.Id == updatedAppartment.Id);
                if (appartment is null)
                {
                    throw new Exception($"Appartment with ID '{updatedAppartment.Id}' not found");
                }

                _mapper.Map(updatedAppartment, appartment);
                await _context.SaveChangesAsync();
                serviceResponse.Data = _mapper.Map<GetAppartmentDto>(appartment);
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
