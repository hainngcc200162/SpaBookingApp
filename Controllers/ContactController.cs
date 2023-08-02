using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SpaBookingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<ContactDto>>> AddContact(ContactDto newContact)
        {
            return Ok(await _contactService.AddContact(newContact));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<ContactDto>>> GetSingle(int id)
        {
            return Ok(await _contactService.GetContactById(id));
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<ServiceResponse<List<ContactDto>>>> GetAll(int pageIndex)
        {

            var serviceResponse = await _contactService.GetAllContacts(pageIndex);

            return Ok(serviceResponse);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<ContactDto>>> DeleteContact(int id)
        {
            var response = await _contactService.DeleteContact(id);
            if (response.Data is null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}
