using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SpaBookingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<ServiceResponse<List<GetSubjectDto>>>> GetAll()
        {
            return Ok(await _subjectService.GetAllSubjects());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<GetSubjectDto>>> GetSingle(int id)
        {
            return Ok(await _subjectService.GetSubjectById(id));
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<GetSubjectDto>>> AddSubject(AddSubjectDto newSubject)
        {
            return Ok(await _subjectService.AddSubject(newSubject));
        }

        [HttpPut]
        public async Task<ActionResult<ServiceResponse<GetSubjectDto>>> UpdateSubject(UpdateSubjectDto updatedSubject)
        {
            var response = await _subjectService.UpdateSubject(updatedSubject);
            if (response.Data is null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<GetSubjectDto>>> DeleteSubject(int id)
        {
            var response = await _subjectService.DeleteSubject(id);
            if (response.Data is null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}
