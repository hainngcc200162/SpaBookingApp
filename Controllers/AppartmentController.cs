using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SpaBookingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppartmentController : ControllerBase
    {
        private readonly IAppartmentService _appartmentService;

        public AppartmentController(IAppartmentService appartmentService)
        {
            _appartmentService = appartmentService;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<ServiceResponse<List<GetAppartmentDto>>>> Get()
        {
            return Ok(await _appartmentService.GetAllAppartments());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<GetAppartmentDto>>> GetSingle(int id)
        {
            return Ok(await _appartmentService.GetAppartmentById(id));
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<GetAppartmentDto>>>> AddAppartment(AddAppartmentDto newAppartment)
        {
            return Ok(await _appartmentService.AddAppartment(newAppartment));
        }

        [HttpPut]
        public async Task<ActionResult<ServiceResponse<GetAppartmentDto>>> UpdateAppartment(UpdateAppartmentDto updatedAppartment)
        {
            var response = await _appartmentService.UpdateAppartment(updatedAppartment);
            if (response.Data is null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<GetAppartmentDto>>> DeleteAppartment(int id)
        {
            var response = await _appartmentService.DeleteAppartment(id);
            if (response.Data is null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}
