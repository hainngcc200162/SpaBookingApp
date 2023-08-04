using System.Collections.Generic;
using System.Threading.Tasks;


namespace SpaBookingApp.Services.BookingService
{
    public interface IBookingService
    {
        Task<ServiceResponse<List<GetBookingDto>>> GetAllBookings();
        Task<ServiceResponse<GetBookingDto>> GetBookingById(int id);
        Task<ServiceResponse<int>> AddBooking(AddBookingDto newBooking);
        Task<ServiceResponse<GetBookingDto>> UpdateBooking(UpdateBookingDto updatedBooking);
        Task<ServiceResponse<GetBookingDto>> DeleteBooking(int id);
    }
}
