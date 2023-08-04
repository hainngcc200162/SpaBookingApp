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

        public async Task<ServiceResponse<int>> AddBooking(AddBookingDto newBooking)
        {
            var serviceResponse = new ServiceResponse<int>();
            var booking = _mapper.Map<Booking>(newBooking);

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

        public async Task<ServiceResponse<List<GetBookingDto>>> GetAllBookings()
        {
            var serviceResponse = new ServiceResponse<List<GetBookingDto>>();
            var dbBookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Provision)
                .Include(b => b.Appartment)
                .Include(b => b.Staff)
                .ToListAsync();

            serviceResponse.Data = dbBookings.Select(b => _mapper.Map<GetBookingDto>(b)).ToList();

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetBookingDto>> GetBookingById(int id)
        {
            var serviceResponse = new ServiceResponse<GetBookingDto>();
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
    }
}
