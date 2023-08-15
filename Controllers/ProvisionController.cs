using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SpaBookingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProvisionController : ControllerBase
    {
         private readonly IProvisionService _provisionService;

        public ProvisionController(IProvisionService provisionService)
        {
            _provisionService = provisionService;
        }

       
        [HttpGet("GetAll")]
        public async Task<ActionResult<List<GetProvisionDto>>> Get()
        {
            return Ok(await _provisionService.GetAllProvisions());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<GetProvisionDto>>> GetSingle(int id)
        {
            return Ok(await _provisionService.GetProvisionById(id));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<GetProvisionDto>>>> AddProvision([FromForm] AddProvisionDto newProvision)
        {
            return Ok(await _provisionService.AddProvision(newProvision));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ServiceResponse<List<GetProvisionDto>>>> UpdateProvision([FromForm] UpdateProvisionDto updatedProvision)
        {
            var response = await _provisionService.UpdateProvision(updatedProvision);
            if (response.Data is null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<GetProvisionDto>>> DeleteProvision(int id)
        {
            var response = await _provisionService.DeleteProvision(id);
            if (response.Data is null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}