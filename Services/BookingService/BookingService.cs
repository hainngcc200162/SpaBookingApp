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
            booking.UserId = userId;

            foreach (var provisionId in newBooking.ProvisionIds)
            {
                booking.ProvisionBookings.Add(new ProvisionBooking { ProvisionId = provisionId });
            }

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

        public async Task<ServiceResponse<List<GetBookingDto>>> GetAllBookings(int userId, int pageIndex, string? searchBy, DateTime? fromDate, DateTime? toDate)
        {
            int pageSize = 4;
            var serviceResponse = new ServiceResponse<List<GetBookingDto>>();

            // Fetch user's role
            string role = _context.Users.Find(userId)?.Role.ToString() ?? "";

            IQueryable<Booking> query = _context.Bookings
                .Include(b => b.User)
                .Include(b => b.ProvisionBookings)
                    .ThenInclude(pb => pb.Provision)
                .Include(b => b.Department)
                .Include(b => b.Staff);

            if (role != "Admin")
            {
                query = query.Where(b => b.UserId == userId);
            }

            // Filter by search criteria
            if (!string.IsNullOrEmpty(searchBy))
            {
                query = query.Where(b =>
                    b.User.FirstName.Contains(searchBy) ||
                    b.User.LastName.Contains(searchBy) ||
                    // b.User.Email.Contains(searchBy) ||
                    b.User.PhoneNumber.Contains(searchBy)
                // b.Department.Name.Contains(searchBy) ||
                // b.Staff.Name.Contains(searchBy) ||
                // b.Note.Contains(searchBy)
                );
            }

            // Filter by date range
            if (fromDate.HasValue && toDate.HasValue)
            {
                query = query.Where(b => b.StartTime.Date >= fromDate.Value.Date && b.StartTime.Date <= toDate.Value.Date);
            }
            else if (fromDate.HasValue)
            {
                // If only fromDate is provided, consider bookings from that day onwards
                query = query.Where(b => b.StartTime.Date >= fromDate.Value.Date);
            }
            else if (toDate.HasValue)
            {
                // If only toDate is provided, consider bookings up to that day
                query = query.Where(b => b.StartTime.Date <= toDate.Value.Date);
            }

            query = query.OrderByDescending(o => o.Id);

            var totalCount = await query.CountAsync();

            var pagedBookings = await query
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Mapping Booking entities to GetBookingDto
            serviceResponse.Data = pagedBookings.Select(b => new GetBookingDto
            {
                Id = b.Id,
                UserId = b.UserId,
                UserFirstName = b.User.FirstName,
                UserLastName = b.User.LastName,
                UserEmail = b.User.Email,
                UserPhoneNumber = b.User.PhoneNumber,
                DepartmentId = b.DepartmentId,
                DepartmentName = b.Department.Name,
                StaffId = b.StaffId,
                StaffName = b.Staff.Name,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                Status = b.Status,
                Note = b.Note,
                Provisions = b.ProvisionBookings.Select(pb => new GetProvisionDto
                {
                    Id = pb.ProvisionId,
                    Name = pb.Provision?.Name,
                    Description = pb.Provision?.Description,
                    Price = pb.Provision?.Price ?? 0,
                    DurationMinutes = pb.Provision?.DurationMinutes ?? 0,
                    Status = pb.Provision?.Status ?? false,
                    PosterName = pb.Provision?.PosterName
                }).ToList()
            }).ToList();

            // Create PageInformation
            var pageInfo = new PageInformation
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            serviceResponse.PageInformation = pageInfo;

            return serviceResponse;
        }


        public async Task<ServiceResponse<GetBookingDto>> GetBookingById(int id, int userId)
        {
            var serviceResponse = new ServiceResponse<GetBookingDto>();

            // Fetch user's role
            string role = _context.Users.Find(userId)?.Role.ToString() ?? "";

            var dbBooking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.ProvisionBookings)
                    .ThenInclude(pb => pb.Provision)
                .Include(b => b.Department)
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

            // Mapping Booking entity to GetBookingDto
            serviceResponse.Data = new GetBookingDto
            {
                Id = dbBooking.Id,
                UserId = dbBooking.UserId,
                UserFirstName = dbBooking.User.FirstName,
                UserLastName = dbBooking.User.LastName,
                UserEmail = dbBooking.User.Email,
                UserPhoneNumber = dbBooking.User.PhoneNumber,
                DepartmentId = dbBooking.DepartmentId,
                DepartmentName = dbBooking.Department.Name,
                StaffId = dbBooking.StaffId,
                StaffName = dbBooking.Staff.Name,
                StartTime = dbBooking.StartTime,
                EndTime = dbBooking.EndTime,
                Status = dbBooking.Status,
                Note = dbBooking.Note,
                Provisions = dbBooking.ProvisionBookings.Select(pb => new GetProvisionDto
                {
                    Id = pb.ProvisionId,
                    Name = pb.Provision?.Name,
                    Description = pb.Provision?.Description,
                    Price = pb.Provision?.Price ?? 0,
                    DurationMinutes = pb.Provision?.DurationMinutes ?? 0,
                    Status = pb.Provision?.Status ?? false,
                    PosterName = pb.Provision?.PosterName
                }).ToList()
            };

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetBookingDto>> UpdateBooking(UpdateBookingDto updatedBooking)
        {
            var serviceResponse = new ServiceResponse<GetBookingDto>();
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.ProvisionBookings)
                        .ThenInclude(pb => pb.Provision)
                    .FirstOrDefaultAsync(b => b.Id == updatedBooking.Id);

                if (booking is null)
                {
                    throw new Exception($"Booking with ID '{updatedBooking.Id}' not found");
                }

                // Update specific booking information
                booking.DepartmentId = updatedBooking.DepartmentId;
                booking.StaffId = updatedBooking.StaffId;
                booking.StartTime = updatedBooking.StartTime;
                booking.EndTime = updatedBooking.EndTime;
                booking.Status = updatedBooking.Status;
                booking.Note = updatedBooking.Note;

                // Update related provisions if needed
                if (updatedBooking.ProvisionIds != null)
                {
                    // Clear existing provisions
                    booking.ProvisionBookings.Clear();

                    // Add new provisions based on provided ProvisionIds
                    foreach (var provisionId in updatedBooking.ProvisionIds)
                    {
                        var provision = await _context.Provisions.FirstOrDefaultAsync(p => p.Id == provisionId);
                        if (provision != null)
                        {
                            booking.ProvisionBookings.Add(new ProvisionBooking
                            {
                                ProvisionId = provision.Id
                            });
                        }
                    }
                }

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

        public async Task<ServiceResponse<bool>> UpdateBookingByCus(int userId, int bookingId, List<int> provisionIds, int departmentId, int staffId, DateTime startTime, DateTime endTime, string note)
        {
            var response = new ServiceResponse<bool>();

            // Fetch user's role
            string role = _context.Users.Find(userId)?.Role.ToString() ?? "";

            var dbBooking = await _context.Bookings
                .Include(b => b.ProvisionBookings)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

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

            if (dbBooking.Status != "waiting")
            {
                response.Success = false;
                response.Message = "Booking status is not 'waiting', cannot update.";
                return response;
            }

            if (provisionIds != null)
            {
                dbBooking.ProvisionBookings.Clear(); // Remove existing provisions

                foreach (var provisionId in provisionIds)
                {
                    var provision = await _context.Provisions.FirstOrDefaultAsync(p => p.Id == provisionId);
                    if (provision == null)
                    {
                        response.Success = false;
                        response.Message = $"Provision with ID '{provisionId}' not found.";
                        return response;
                    }

                    var existingProvisionBooking = dbBooking.ProvisionBookings.FirstOrDefault(pb => pb.ProvisionId == provisionId);
                    if (existingProvisionBooking != null)
                    {
                        // Cập nhật thông tin của ProvisionBooking đã tồn tại
                        existingProvisionBooking.ProvisionId = provisionId;
                    }
                    else
                    {
                        dbBooking.ProvisionBookings.Add(new ProvisionBooking
                        {
                            ProvisionId = provision.Id
                        });
                    }
                }
            }

            if (departmentId != 0)
            {
                dbBooking.DepartmentId = departmentId;
            }

            if (staffId != 0)
            {
                dbBooking.StaffId = staffId;
            }

            dbBooking.StartTime = startTime;
            dbBooking.EndTime = endTime;

            if (!string.IsNullOrEmpty(note))
            {
                dbBooking.Note = note;
            }

            _context.Bookings.Update(dbBooking);
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Message = "Booking updated successfully.";
            response.Data = true;

            return response;
        }
    }
}
