using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SpaBookingApp.Data;

namespace SpaBookingApp.Services.BookingService
{
    public class BookingService : IBookingService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public BookingService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<ServiceResponse<int>> AddBooking(int userId, AddBookingDto newBooking)
        {
            var serviceResponse = new ServiceResponse<int>();

            var booking = _mapper.Map<Booking>(newBooking);
            booking.UserId = userId; // Liên kết booking với người dùng

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            serviceResponse.Data = booking.Id;
            serviceResponse.Success = true;
            serviceResponse.Message = "Booking created successfully.";

            return serviceResponse;
        }


        public async Task<ServiceResponse<GetBookingDto>> DeleteBooking(int id)
        {
            var serviceResponse = new ServiceResponse<GetBookingDto>();
            try
            {
                var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id);
                if (booking is null)
                {
                    throw new Exception($"Booking with ID '{id}' not found");
                }

                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();

                serviceResponse.Data = _mapper.Map<GetBookingDto>(booking);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetBookingDto>>> GetAllBookings(int userId)
        {
            var serviceResponse = new ServiceResponse<List<GetBookingDto>>();

            // Fetch user's role
            string role = _context.Users.Find(userId)?.Role.ToString() ?? "";

            IQueryable<Booking> query = _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Provision)
                .Include(b => b.Appartment)
                .Include(b => b.Staff);

            if (role != "Admin")
            {
                query = query.Where(b => b.UserId == userId);
            }

            var dbBookings = await query.ToListAsync();

            serviceResponse.Data = dbBookings.Select(b => _mapper.Map<GetBookingDto>(b)).ToList();

            return serviceResponse;
        }


        public async Task<ServiceResponse<GetBookingDto>> GetBookingById(int id, int userId)
        {
            var serviceResponse = new ServiceResponse<GetBookingDto>();

            // Fetch user's role
            string role = _context.Users.Find(userId)?.Role.ToString() ?? "";

            var dbBooking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Provision)
                .Include(b => b.Appartment)
                .Include(b => b.Staff)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (dbBooking is null)
            {
                throw new Exception($"Booking with ID '{id}' not found");
            }

            if (role != "Admin" && dbBooking.UserId != userId)
            {
                throw new UnauthorizedAccessException("You do not have permission to access this booking.");
            }

            serviceResponse.Data = _mapper.Map<GetBookingDto>(dbBooking);
            return serviceResponse;
        }


        public async Task<ServiceResponse<GetBookingDto>> UpdateBooking(UpdateBookingDto updatedBooking)
        {
            var serviceResponse = new ServiceResponse<GetBookingDto>();
            try
            {
                var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == updatedBooking.Id);
                if (booking is null)
                {
                    throw new Exception($"Booking with ID '{updatedBooking.Id}' not found");
                }

                _mapper.Map(updatedBooking, booking);

                await _context.SaveChangesAsync();
                serviceResponse.Data = _mapper.Map<GetBookingDto>(booking);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<bool>> UpdateBookingByCus(int userId, int bookingId, int provisionId, int appartmentId, int staffId, DateTime startTime, DateTime endTime, string note)
        {
            var response = new ServiceResponse<bool>();

            // Fetch user's role
            string role = _context.Users.Find(userId)?.Role.ToString() ?? "";

            var dbBooking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);

            if (dbBooking == null)
            {
                response.Success = false;
                response.Message = $"Booking with ID '{bookingId}' not found.";
                return response;
            }

            if (dbBooking.UserId != userId)
            {
                response.Success = false;
                response.Message = "You do not have permission to update this booking.";
                return response;
            }

            if (dbBooking.Status != "Waiting")
            {
                response.Success = false;
                response.Message = "Booking status is not 'waiting', cannot update.";
                return response;
            }

            // Update booking properties
            dbBooking.ProvisionId = provisionId;
            dbBooking.AppartmentId = appartmentId;
            dbBooking.StaffId = staffId;
            dbBooking.StartTime = startTime;
            dbBooking.EndTime = endTime;
            dbBooking.Note = note;

            _context.Bookings.Update(dbBooking);
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Message = "Booking updated successfully.";
            response.Data = true;

            return response;
        }
    }
}
