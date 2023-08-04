using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpaBookingApp.Dtos.Booking;
using SpaBookingApp.Services.BookingService;

namespace SpaBookingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<ServiceResponse<List<GetBookingDto>>>> GetAll()
        {
            var response = await _bookingService.GetAllBookings();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<GetBookingDto>>> GetSingle(int id)
        {
            var response = await _bookingService.GetBookingById(id);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<int>>> AddBooking(AddBookingDto newBooking)
        {
            var response = await _bookingService.AddBooking(newBooking);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPut]
        public async Task<ActionResult<ServiceResponse<GetBookingDto>>> UpdateBooking(UpdateBookingDto updatedBooking)
        {
            var response = await _bookingService.UpdateBooking(updatedBooking);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<GetBookingDto>>> DeleteBooking(int id)
        {
            var response = await _bookingService.DeleteBooking(id);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}
