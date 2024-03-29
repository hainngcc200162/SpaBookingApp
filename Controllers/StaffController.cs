using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


namespace SpaBookingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<ServiceResponse<List<GetStaffDto>>>> Get(int pageIndex,int pageSize, string searchByName,StaffGender? searchByGender)
        {
            return Ok(await _staffService.GetAllStaffs(pageIndex, pageSize, searchByName, searchByGender));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<GetStaffDto>>> GetSingle(int id)
        {
            return Ok(await _staffService.GetStaffById(id));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<GetStaffDto>>>> AddStaff([FromForm] AddStaffDto newStaff)
        {
            return Ok(await _staffService.AddStaff(newStaff));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ServiceResponse<GetStaffDto>>> UpdateStaff([FromForm] UpdateStaffDto updatedStaff)
        {
            var response = await _staffService.UpdateStaff(updatedStaff);
            if (response.Data is null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<GetStaffDto>>> DeleteStaff(int id)
        {
            var response = await _staffService.DeleteStaff(id);
            if (response.Data is null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}
