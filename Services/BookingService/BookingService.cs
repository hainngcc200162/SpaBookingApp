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
        private readonly IEmailService _emailService;

        public BookingService(IMapper mapper, IEmailService emailService, DataContext context)
        {
            _mapper = mapper;
            _context = context;
            _emailService = emailService;
        }

        public async Task<ServiceResponse<int>> AddBooking(int userId, AddBookingDto newBooking)
        {
            var serviceResponse = new ServiceResponse<int>();

            // Kiểm tra xem nhân viên đã có lịch làm việc trong khoảng thời gian mới đặt lịch chưa
            var isStaffAvailable = IsStaffAvailable(newBooking.StaffId, newBooking.StartTime, newBooking.EndTime, 1);

            if (!isStaffAvailable)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Please choose another time, this employee has an appointment scheduled at this time";
                return serviceResponse;
            }

            if (newBooking.StartTime <= DateTime.Now.AddHours(1))
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Cannot be booked for a period of time less than 1 hour from the current time.";
                return serviceResponse;
            }

            var booking = _mapper.Map<Booking>(newBooking);
            booking.UserId = userId;

            var totalDuration = TimeSpan.Zero;

            bool atLeastOneProvisionWithTrueStatus = false;

            foreach (var provisionId in newBooking.ProvisionIds)
            {
                var provision = _context.Provisions.FirstOrDefault(p => p.Id == provisionId);

                if (provision != null)
                {
                    if (provision.Status)
                    {
                        atLeastOneProvisionWithTrueStatus = true;
                        totalDuration += TimeSpan.FromMinutes(provision.DurationMinutes);
                        int numberOfExecutions = provision.NumberOfExecutions;

                        var provisionBooking = new ProvisionBooking
                        {
                            ProvisionId = provisionId,
                            RemainingExecutions = numberOfExecutions
                        };

                        booking.ProvisionBookings.Add(provisionBooking);
                    }
                    else
                    {
                        // Handle the case where a provision has a false status
                        serviceResponse.Success = false;
                        serviceResponse.Message = $"{provision.Name} service cannot be booked because it has a Inactive status.";
                        return serviceResponse;
                    }
                }
                else
                {
                    // Handle the case where the provision does not exist
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Provision does not exist.";
                }
            }

            if (!atLeastOneProvisionWithTrueStatus)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "No provision can be booked because none of them have a true status.";
                return serviceResponse;
            }


            // Tính toán thời gian kết thúc dựa trên tổng thời gian của tất cả các dịch vụ
            booking.EndTime = newBooking.StartTime.Add(totalDuration);

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            serviceResponse.Data = booking.Id;
            serviceResponse.Success = true;
            serviceResponse.Message = "Booking created successfully.";

            int newBookingId = booking.Id;
            var getBookingResponse = await GetBookingById(newBookingId, userId);

            if (!getBookingResponse.Success)
            {
                // Xử lý lỗi nếu không tìm thấy booking
                // ...
            }
            else
            {
                // Lấy thông tin chi tiết của booking từ getBookingResponse.Data
                var detailedBooking = getBookingResponse.Data;

                // Gọi phương thức để gửi email với thông tin chi tiết của booking
                Task.Run(() => SendEmailWithBookingDetails(detailedBooking));
            }

            return serviceResponse;
        }

        private async Task SendEmailWithBookingDetails(GetBookingDto detailedBooking)
        {
            string email = detailedBooking.UserEmail;
            string subject = "Booking Details";
            string startTime = detailedBooking.StartTime.ToString("dd/MM/yyyy HH:mm");
            string endTime = detailedBooking.EndTime.ToString("dd/MM/yyyy HH:mm");

            // Tạo nội dung email với định dạng HTML
            string body = $"<p>Hello {detailedBooking.UserFirstName} {detailedBooking.UserLastName},</p>" +
                          $"<p>Your booking details are as follows:</p>" +
                          $"<ul>" +
                          $"<li><strong>Start Time:</strong> {startTime}</li>" +
                          $"<li><strong>End Time:</strong> {endTime}</li>" +
                          $"<li><strong>Department:</strong> {detailedBooking.DepartmentName}</li>" +
                          $"<li><strong>Staff:</strong> {detailedBooking.StaffName}</li>" +
                          $"<li><strong>Note:</strong> {detailedBooking.Note}</li>" +
                          $"</ul>" +
                          $"<p>Thank you for using our SpaBookingApp!</p>";

            MailRequest mailRequest = new MailRequest
            {
                ToEmail = email,
                Subject = subject,
                Body = body
            };

            await _emailService.SendEmailAsync(mailRequest);
        }

        // Hàm kiểm tra sự sẵn có của nhân viên trong khoảng thời gian
        private bool IsStaffAvailable(int staffId, DateTime startTime, DateTime endTime, int excludeStaffId)
        {
            // Kiểm tra xem có lịch làm việc nào của nhân viên trong khoảng thời gian này không,
            // trừ nhân viên có ID là excludeStaffId
            var existingBooking = _context.Bookings.FirstOrDefault(b =>
                b.StaffId != excludeStaffId && // Không tính nhân viên có excludeStaffId
                b.StaffId == staffId &&
                ((startTime >= b.StartTime && startTime < b.EndTime) ||
                (endTime > b.StartTime && endTime <= b.EndTime)));

            return existingBooking == null;
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

        public async Task<int> DeleteCancelledBookings()
        {
            // Lấy danh sách các booking có trạng thái là "Cancelled"
            var cancelledBookings = await _context.Bookings
                .Where(b => b.Status == "Cancelled")
                .ToListAsync();

            // Xoá các booking đã lấy
            _context.Bookings.RemoveRange(cancelledBookings);

            // Lưu các thay đổi vào cơ sở dữ liệu
            return await _context.SaveChangesAsync();
        }

        public async Task<ServiceResponse<List<GetBookingDto>>> GetAllBookings(int userId, int pageIndex, string? searchBy, DateTime? fromDate, DateTime? toDate)
        {
            int pageSize = 20;
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
                    b.User.Email.Contains(searchBy) ||
                    b.User.PhoneNumber.Contains(searchBy)

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
                    NumberOfExecutions = pb.Provision?.NumberOfExecutions ?? 0,
                    Status = pb.Provision?.Status ?? false,
                    PosterName = pb.Provision?.PosterName,

                    RemainingExecutions = pb.RemainingExecutions
                }).ToList(),

                // Lấy danh sách RemainingExecutions của các provisions trong booking
                ProvisionRemainingExecutions = b.ProvisionBookings.Select(pb => pb.RemainingExecutions).ToList()
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
                    NumberOfExecutions = pb.Provision?.NumberOfExecutions ?? 0,
                    Status = pb.Provision?.Status ?? false,
                    PosterName = pb.Provision?.PosterName,
                    RemainingExecutions = pb.RemainingExecutions // Lấy RemainingExecutions từ ProvisionBooking
                }).ToList(),

                // Lấy danh sách ProvisionRemainingExecutions của các provisions trong booking
                ProvisionRemainingExecutions = dbBooking.ProvisionBookings.Select(pb => pb.RemainingExecutions).ToList()
            };

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetBookingDto>> UpdateBooking(int id, UpdateBookingDto updatedBooking)
        {
            var serviceResponse = new ServiceResponse<GetBookingDto>();

            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                var booking = await _context.Bookings
                    .Include(b => b.ProvisionBookings)
                        .ThenInclude(pb => pb.Provision)
                    .Include(b => b.User)
                    .Include(b => b.Department)
                    .Include(b => b.Staff)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (booking == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "No bookings found.";
                    return serviceResponse;
                }

                // Cập nhật thông tin đặt lịch
                _mapper.Map(updatedBooking, booking);

                var provisionsChanged = !booking.ProvisionBookings.Select(pb => pb.ProvisionId).SequenceEqual(updatedBooking.ProvisionIds);

                if (provisionsChanged)
                {
                    // Xóa dịch vụ hiện có
                    booking.ProvisionBookings.Clear();

                    // Tính tổng thời gian và cập nhật dịch vụ
                    var totalDuration = TimeSpan.Zero;
                    foreach (var provisionId in updatedBooking.ProvisionIds)
                    {
                        var provision = await _context.Provisions.FirstOrDefaultAsync(p => p.Id == provisionId);
                        if (provision != null)
                        {
                            totalDuration += TimeSpan.FromMinutes(provision.DurationMinutes);
                            booking.ProvisionBookings.Add(new ProvisionBooking { ProvisionId = provisionId });
                        }
                    }

                    // Cập nhật thời gian kết thúc dựa trên thời gian bắt đầu và tổng thời gian
                    booking.EndTime = updatedBooking.StartTime.Add(totalDuration);
                }
                else
                {
                    // Cập nhật thời gian kết thúc dựa trên thời gian bắt đầu người dùng đã nhập và tổng thời gian dịch vụ
                    var totalDuration = booking.ProvisionBookings.Sum(pb => pb.Provision.DurationMinutes);
                    booking.EndTime = updatedBooking.StartTime.AddMinutes(totalDuration);
                }

                // Cập nhật RemainingExecutions cho các dịch vụ
                foreach (var provisionId in updatedBooking.ProvisionIds)
                {
                    // Tìm dịch vụ tương ứng
                    var provision = await _context.Provisions.FirstOrDefaultAsync(p => p.Id == provisionId);
                    if (provision != null)
                    {
                        // Kiểm tra xem RemainingExecutions hợp lệ
                        var remainingExecutions = updatedBooking.ProvisionRemainingExecutions.FirstOrDefault(re => re.ProvisionId == provisionId);

                        if (remainingExecutions != null)
                        {
                            if (remainingExecutions.RemainingExecutions >= 0 && remainingExecutions.RemainingExecutions <= provision.NumberOfExecutions)
                            {
                                // Lấy provision trong booking
                                var provisionBooking = booking.ProvisionBookings.FirstOrDefault(pb => pb.ProvisionId == provisionId);

                                if (provisionBooking != null)
                                {
                                    // Cập nhật RemainingExecutions cho provision
                                    provisionBooking.RemainingExecutions = remainingExecutions.RemainingExecutions;
                                }
                                else
                                {
                                    serviceResponse.Success = false;
                                    serviceResponse.Message = "Provision Not Found.";

                                }
                            }
                            else
                            {
                                serviceResponse.Success = false;
                                // Xử lý trường hợp RemainingExecutions không hợp lệ
                                serviceResponse.Message = "Remaining Executions phải nhỏ hơn Number excution và lớn hơn hoặc bằng 0 ).";
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Tạo DTO phản hồi
                var responseDto = _mapper.Map<GetBookingDto>(booking);

                // Ánh xạ dữ liệu dịch vụ vào DTO phản hồi
                responseDto.Provisions = booking.ProvisionBookings.Select(pb =>
                {
                    var provisionDto = _mapper.Map<GetProvisionDto>(pb.Provision);
                    provisionDto.DurationMinutes = pb.Provision.DurationMinutes; // Set the DurationMinutes based on the provision
                    return provisionDto;
                }).ToList();

                serviceResponse.Data = responseDto;
                Task.Run(() => SendBookingStatusUpdateEmailAsync(responseDto));
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }


        public async Task<ServiceResponse<bool>> UpdateBookingByCus(int userId, int bookingId, List<int> provisionIds, int departmentId, int staffId, DateTime startTime, DateTime endTime, string status, string note)
        {
            var response = new ServiceResponse<bool>();

            // Fetch user's role
            string role = _context.Users.Find(userId)?.Role.ToString() ?? "";

            var dbBooking = await _context.Bookings
                .Include(b => b.ProvisionBookings)
                .Include(b => b.Department)  // Include the Department navigation property
                .Include(b => b.Staff)      // Include the Staff navigation property
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

            if (DateTime.Now.AddHours(1) >= startTime)
            {
                response.Success = false;
                response.Message = "Booking cannot be updated as it is less than 1 hour before the start time.";
                return response;
            }

            // Load user data
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            // Cập nhật trạng thái của đơn đặt hàng
            dbBooking.Status = status;

            if (provisionIds != null)
            {
                dbBooking.ProvisionBookings.Clear(); // Remove existing provisions

                var totalDuration = TimeSpan.Zero;
                foreach (var provisionId in provisionIds)
                {
                    var provision = await _context.Provisions.FirstOrDefaultAsync(p => p.Id == provisionId);
                    if (provision == null)
                    {
                        response.Success = false;
                        response.Message = $"Provision with ID '{provisionId}' not found.";
                        return response;
                    }

                    dbBooking.ProvisionBookings.Add(new ProvisionBooking
                    {
                        ProvisionId = provision.Id
                    });

                    totalDuration += TimeSpan.FromMinutes(provision.DurationMinutes);
                }

                // Cập nhật thời gian kết thúc dựa trên thời gian bắt đầu và tổng thời gian dịch vụ
                dbBooking.EndTime = startTime.Add(totalDuration);
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

            if (!string.IsNullOrEmpty(note))
            {
                dbBooking.Note = note;
            }

            _context.Bookings.Update(dbBooking);
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Message = "Booking updated successfully.";
            response.Data = true;

            // Pass user and booking data to the email function
            Task.Run(() => SendBookingStatusUpdateEmaillAsync(user, dbBooking));

            return response;
        }


        private async Task SendBookingStatusUpdateEmaillAsync(User user, Booking booking)
        {
            string email = user.Email;
            string subject = "Booking Status Update";
            string status = booking.Status;
            string startTime = booking.StartTime.ToString("dd/MM/yyyy HH:mm");
            string endTime = booking.EndTime.ToString("dd/MM/yyyy HH:mm");

            // Tạo nội dung email với định dạng HTML
            string body = $"<p>Hello {user.FirstName} {user.LastName},</p>" +
                        $"<p>Your booking with the following details has been updated:</p>" +
                        $"<ul>" +
                        $"<li><strong>Status:</strong> {status}</li>" +
                        $"<li><strong>Start Time:</strong> {startTime}</li>" +
                        $"<li><strong>End Time:</strong> {endTime}</li>" +
                        $"<li><strong>Department:</strong> {booking.Department.Name}</li>" +
                        $"<li><strong>Staff:</strong> {booking.Staff.Name}</li>" +
                        $"<li><strong>Note:</strong> {booking.Note}</li>" +
                        $"</ul>" +
                        $"<p>Thank you for using our SpaBookingApp!</p>";

            MailRequest mailRequest = new MailRequest
            {
                ToEmail = email,
                Subject = subject,
                Body = body
            };

            await _emailService.SendEmailAsync(mailRequest);
        }


        private async Task SendBookingStatusUpdateEmailAsync(GetBookingDto emailBooking)
        {
            string email = emailBooking.UserEmail;
            string subject = "Booking Status Update";
            string status = emailBooking.Status;
            string startTime = emailBooking.StartTime.ToString("dd/MM/yyyy HH:mm");
            string endTime = emailBooking.EndTime.ToString("dd/MM/yyyy HH:mm");

            // Tạo nội dung email với định dạng HTML
            string body = $"<p>Hello {emailBooking.UserFirstName} {emailBooking.UserLastName},</p>" +
                        $"<p>Your booking with the following details has been updated:</p>" +
                        $"<ul>" +
                        $"<li><strong>Status:</strong> {status}</li>" +
                        $"<li><strong>Start Time:</strong> {startTime}</li>" +
                        $"<li><strong>End Time:</strong> {endTime}</li>" +
                        $"<li><strong>Department:</strong> {emailBooking.DepartmentName}</li>" +
                        $"<li><strong>Staff:</strong> {emailBooking.StaffName}</li>" +
                        $"<li><strong>Note:</strong> {emailBooking.Note}</li>" +
                        $"</ul>" +
                        $"<p>Thank you for using our SpaBookingApp!</p>";

            MailRequest mailRequest = new MailRequest
            {
                ToEmail = email,
                Subject = subject,
                Body = body
            };

            await _emailService.SendEmailAsync(mailRequest);
        }
    }
}
