using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


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

        [Authorize]
        [HttpGet("GetAll")]
        public async Task<ActionResult<ServiceResponse<List<GetBookingDto>>>> GetAll(int pageIndex, string? searchBy, DateTime? fromDate, DateTime? toDate)
        {
            int userId = JwtReader.GetUserId(User);

            var serviceResponse = await _bookingService.GetAllBookings(userId, pageIndex, searchBy, fromDate, toDate);
            return Ok(serviceResponse);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<GetBookingDto>>> GetSingle(int id)
        {
            int userId = JwtReader.GetUserId(User);

            var response = await _bookingService.GetBookingById(id, userId);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ServiceResponse<int>>> AddBooking(AddBookingDto newBooking)
        {
            int userId = JwtReader.GetUserId(User);

            if (userId == 0)
            {
                return BadRequest(new ServiceResponse<int>
                {
                    Success = false,
                    Message = "Invalid user."
                });
            }

            var response = await _bookingService.AddBooking(userId, newBooking);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }


        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ServiceResponse<GetBookingDto>>> UpdateBooking(int id, UpdateBookingDto updatedBooking)
        {   
            var response = await _bookingService.UpdateBooking(id, updatedBooking);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Customer")]
        [HttpPut("UpdateBookingByCus/{id}")]
        public async Task<IActionResult> UpdateBookingByCus(int id,
    List<int> provisionIds,
    int departmentId,
    int staffId,
    DateTime startTime,
    DateTime endTime,
    string status,
    string note)
        {
            int userId = JwtReader.GetUserId(User);
            var response = await _bookingService.UpdateBookingByCus(userId, id, provisionIds, departmentId, staffId, startTime, endTime,status, note);

            if (!response.Success)
            {
                return BadRequest(response); // Return error message if booking update is not successful
            }

            return Ok(response); // Return the updated booking if update is successful
        }

        
    }
}
