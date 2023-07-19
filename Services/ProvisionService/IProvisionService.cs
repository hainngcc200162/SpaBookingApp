using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SpaBookingApp.Services.ProvisionService
{
    public interface IProvisionService
    {
        Task<ServiceResponse<List<GetProvisionDto>>> GetAllProvisions();
        Task<ServiceResponse<GetProvisionDto>> GetProvisionById(int id);
        Task<ServiceResponse<List<GetProvisionDto>>> AddProvision([FromForm] AddProvisionDto newProvision);
        Task<ServiceResponse<GetProvisionDto>> UpdateProvision([FromForm] UpdateProvisionDto updatedProvision);
        Task<ServiceResponse<List<GetProvisionDto>>> DeleteProvision(int id);
    }
}