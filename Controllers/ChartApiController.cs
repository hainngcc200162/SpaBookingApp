using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SpaBookingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChartApiController : ControllerBase
    {
        private readonly DataContext _context;
        public ChartApiController(DataContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("orders-and-bookings")]
        public IActionResult GetOrdersAndBookingsData()
        {
            // Truy vấn dữ liệu cho Orders và Bookings
            var ordersData = _context.Orders
                .GroupBy(o => o.CreatedAt.Month)
                .Select(group => new
                {
                    Month = group.Key,
                    TotalOrders = group.Count()
                })
                .OrderBy(data => data.Month)
                .ToList();

            var bookingsData = _context.Bookings
                .GroupBy(b => b.StartTime.Month)
                .Select(group => new
                {
                    Month = group.Key,
                    TotalBookings = group.Count()
                })
                .OrderBy(data => data.Month)
                .ToList();

            // Truy vấn dữ liệu giá trị đặt lịch từ ProvisionBooking
            var provisionBookingData = _context.ProvisionBookings
                .GroupBy(pb => pb.Booking.StartTime.Month)
                .Select(group => new
                {
                    Month = group.Key,
                    TotalBookingRevenue = group.Sum(pb => pb.Provision.Price)
                })
                .OrderBy(data => data.Month)
                .ToList();

            // Truy vấn dữ liệu giá trị đơn hàng từ Orders
            var orderValuesData = _context.Orders
                .Where(o => o.CreatedAt.Year == DateTime.Now.Year)
                .GroupBy(o => o.CreatedAt.Month)
                .Select(group => new
                {
                    Month = group.Key,
                    TotalOrderValue = group.Sum(o => o.TotalPrice)
                })
                .OrderBy(data => data.Month)
                .ToList();

            // Chuẩn bị dữ liệu cho biểu đồ
            var months = Enumerable.Range(1, 12).Select(m => m.ToString()).ToArray();
            var ordersCountByMonth = new int[12];
            var bookingsCountByMonth = new int[12];
            var bookingRevenuesByMonth = new decimal[12];
            var orderValuesByMonth = new decimal[12];

            foreach (var orderData in ordersData)
            {
                ordersCountByMonth[orderData.Month - 1] = orderData.TotalOrders;
            }

            foreach (var bookingData in bookingsData)
            {
                bookingsCountByMonth[bookingData.Month - 1] = bookingData.TotalBookings;
            }

            foreach (var pbData in provisionBookingData)
            {
                bookingRevenuesByMonth[pbData.Month - 1] = pbData.TotalBookingRevenue;
            }

            foreach (var orderValueData in orderValuesData)
            {
                orderValuesByMonth[orderValueData.Month - 1] = orderValueData.TotalOrderValue;
            }

            // Trả về dữ liệu JSON
            return Ok(new
            {
                Months = months,
                OrdersCountByMonth = ordersCountByMonth,
                BookingsCountByMonth = bookingsCountByMonth,
                BookingRevenuesByMonth = bookingRevenuesByMonth,
                OrderValuesByMonth = orderValuesByMonth
            });
        }
    }
}