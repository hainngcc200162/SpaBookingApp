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
    }
}