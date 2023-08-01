using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Services.AppartmentService
{
    public interface IAppartmentService
    {
        Task<ServiceResponse<List<GetAppartmentDto>>> GetAllAppartments();
        Task<ServiceResponse<GetAppartmentDto>> GetAppartmentById(int id);
        Task<ServiceResponse<List<GetAppartmentDto>>> AddAppartment(AddAppartmentDto newAppartment);
        Task<ServiceResponse<GetAppartmentDto>> UpdateAppartment(UpdateAppartmentDto updatedAppartment);
        Task<ServiceResponse<GetAppartmentDto>> DeleteAppartment(int id);
    }
}