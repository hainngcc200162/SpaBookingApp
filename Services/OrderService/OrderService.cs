using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly DataContext _context;

        public OrderService(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<Order>> CreateOrder(int userId, OrderDto orderDto)
        {
            var response = new ServiceResponse<Order>();

            //check if the payment method is valid or not
            if (!OrderHelper.PaymentMethods.ContainsKey(orderDto.PaymentMethod))
            {
                response.Success = false;
                response.Message = "Please select a valid payment method";
                return response;
            }

            var user = _context.Users.Find(userId);
            if (user == null)
            {
                response.Success = false;
                response.Message = "Unable to create the order";
                return response;
            }

            var productDictionary = OrderHelper.GetProductDictionary(orderDto.ProductIdentifiers);

            //Create a new order
            Order order = new Order();
            order.UserId = userId;
            order.CreatedAt = DateTime.Now;
            order.ShippingFee = OrderHelper.ShippingFee;
            order.DeliveryAddress = orderDto.DeliveryAddress;
            order.PaymentMethod = orderDto.PaymentMethod;
            order.PaymentStatus = OrderHelper.PaymentStatuses[0]; //pending
            order.OrderStatus = OrderHelper.OrderStatuses[0]; //created

            foreach (var pair in productDictionary)
            {
                int productId = pair.Key;
                int orderedQuantity = pair.Value;

                var product = _context.Products.Find(productId);
                if (product == null)
                {
                    response.Success = false;
                    response.Message = "Product with ID " + productId + " is not available";
                    return response;
                }

                // Check if there is enough quantity in stock for the product
                if (product.QuantityInStock < orderedQuantity)
                {
                    response.Success = false;
                    response.Message = "Insufficient quantity in stock for Product ID " + productId;
                    return response;
                }

                var orderItem = new OrderItem();
                orderItem.ProductId = productId;
                orderItem.Quantity = pair.Value;
                orderItem.UnitPrice = product.Price;

                // Update the quantity in stock for the product
                product.QuantityInStock -= orderedQuantity;

                order.OrderItems.Add(orderItem);
            }

            if (order.OrderItems.Count < 1)
            {
                response.Success = false;
                response.Message = "Unable to create order";
                return response;
            }

            //save in the database
            _context.Orders.Add(order);
            _context.SaveChanges();

            //get rid of the object cycle
            foreach (var item in order.OrderItems)
            {
                item.Order = null;
            }

             //hide the user password
            order.User.PasswordHash = null;
            order.User.PasswordSalt = null;




            response.Data = order;
            response.Message = "Order created successfully.";
            return response;
        }
    }
}