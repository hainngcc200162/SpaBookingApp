using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SpaBookingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _DepartmentService;

        public DepartmentController(IDepartmentService DepartmentService)
        {
            _DepartmentService = DepartmentService;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<ServiceResponse<List<GetDepartmentDto>>>> Get(int pageIndex, string searchByName)
        {
            return Ok(await _DepartmentService.GetAllDepartments(pageIndex, searchByName));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<GetDepartmentDto>>> GetSingle(int id)
        {
            return Ok(await _DepartmentService.GetDepartmentById(id));
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<GetDepartmentDto>>>> AddDepartment(AddDepartmentDto newDepartment)
        {
            return Ok(await _DepartmentService.AddDepartment(newDepartment));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ServiceResponse<GetDepartmentDto>>> UpdateDepartment(UpdateDepartmentDto updatedDepartment)
        {
            var response = await _DepartmentService.UpdateDepartment(updatedDepartment);
            if (response.Data is null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<GetDepartmentDto>>> DeleteDepartment(int id)
        {
            var response = await _DepartmentService.DeleteDepartment(id);
            if (response.Data is null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}
