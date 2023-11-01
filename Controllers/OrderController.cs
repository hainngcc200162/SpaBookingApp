using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SpaBookingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderDto orderDto)
        {
            int userId = JwtReader.GetUserId(User);

            if (userId == 0)
            {
                return BadRequest(new ServiceResponse<Order>
                {
                    Success = false,
                    Message = "Invalid user."
                });
            }

            var response = await _orderService.CreateOrder(userId, orderDto);

            if (!response.Success)
            {
                return BadRequest(response); // Return error message if order creation is not successful
            }

            return Ok(response); // Return order details if creation is successful
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetOrders(int pageIndex,int pageSize, string searchPhoneNumber, DateTime? fromDate, DateTime? toDate, string searchStripeSessionId, string searchPaymentMethod)
        {
            int userId = JwtReader.GetUserId(User);

            // Call the order service to get the orders for the specified page index
            var response = await _orderService.GetOrders(userId, pageIndex, pageSize, searchPhoneNumber, fromDate, toDate, searchStripeSessionId, searchPaymentMethod );

            // Return the ServiceResponse<List<Order>> containing both orders and page info
            return Ok(response);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSingle(int id)
        {
            int userId = JwtReader.GetUserId(User); // Get the userId from the JWT token

            var response = await _orderService.GetSingleOrder(id, userId);

            if (!response.Success)
            {
                return NotFound(response); // Trả về lỗi nếu không tìm thấy đơn hàng
            }

            return Ok(response.Data); // Trả về thông tin đơn hàng nếu tìm thấy
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, string? paymentStatus, string? orderStatus, string? deliveryAddress, string? phoneNumber)
        {
            // Call the order service to update the order based on user role and permissions
            var response = await _orderService.UpdateOrder(id, paymentStatus, orderStatus, deliveryAddress, phoneNumber);

            if (!response.Success)
            {
                return BadRequest(response); // Return error message if order update is not successful
            }

            return Ok(response); // Return the updated order if update is successful
        }

        [Authorize(Roles = "Customer")]
        [HttpPut("UpdateOrderByCus/{id}")]
        public async Task<IActionResult> UpdateOrderByCus(int id, string? deliveryAddress, string? phoneNumber, string? orderStatus)
        {
            int userId = JwtReader.GetUserId(User);
            var response = await _orderService.UpdateOrderByCus(userId, id, deliveryAddress, phoneNumber, orderStatus);

            if (!response.Success)
            {
                return BadRequest(response); // Return error message if order update is not successful
            }

            return Ok(response); // Return the updated order if update is successful
        }
    }
}