using System.Collections.Generic;
using System.Threading.Tasks;


namespace SpaBookingApp.Services.BookingService
{
    public interface IBookingService
    {
        Task<ServiceResponse<List<GetBookingDto>>> GetAllBookings(int userId, int pageIndex, string? searchBy, DateTime? fromDate, DateTime? toDate);
        Task<ServiceResponse<GetBookingDto>> GetBookingById(int id, int userId);
        Task<ServiceResponse<int>> AddBooking(int userId, AddBookingDto newBooking);
        Task<ServiceResponse<GetBookingDto>> UpdateBooking(UpdateBookingDto updatedBooking);
        Task<ServiceResponse<bool>> UpdateBookingByCus(int userId, int bookingId, List<int> provisionIds, int departmentId, int staffId, DateTime startTime, DateTime endTime, string note);
        Task<ServiceResponse<GetBookingDto>> DeleteBooking(int id);

    }
}
