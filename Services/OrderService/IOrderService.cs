using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Services.OrderService
{
    public interface IOrderService
    {
        Task<ServiceResponse<Order>> CreateOrder(int userId, OrderDto orderDto);
        Task<ServiceResponse<List<Order>>> GetOrders(int userId, int PageIndex);
        Task<ServiceResponse<Order>> GetSingleOrder(int userId, int orderId);
        Task<ServiceResponse<Order>> UpdateOrder(int id, string? paymentStatus, string? orderStatus, string? deliveryAddress, string? phoneNumber);
        Task<ServiceResponse<Order>> UpdateOrderByCus(int userId, int id, string? deliveryAddress, string? phoneNumber);
    }
}