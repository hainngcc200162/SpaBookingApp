using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Services.OrderService
{
    public interface IOrderService
    {
        Task<ServiceResponse<Order>> CreateOrder(int userId, OrderDto orderDto);
    }
}