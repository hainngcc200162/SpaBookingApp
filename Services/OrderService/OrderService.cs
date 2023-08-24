using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace SpaBookingApp.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly DataContext _context;

        private readonly ICartService _cartService;
        private readonly IMemoryCache _cache;


        public OrderService(DataContext context, ICartService cartService, IMemoryCache cache)
        {
            _context = context;
            _cartService = cartService;
            _cache = cache;
        }

        public async Task<ServiceResponse<Order>> CreateOrder(int userId, OrderDto orderDto)
        {
            var response = new ServiceResponse<Order>();

            // Check if the payment method is valid or not
            if (!OrderHelper.PaymentMethods.ContainsKey(orderDto.PaymentMethod))
            {
                response.Success = false;
                response.Message = "Please select a valid payment method";
                return response;
            }
            
            if (!OrderHelper.OrderStatuses.Contains(orderDto.OrderStatus))
            {
                response.Success = false;
                response.Message = "Invalid order status";
                return response;
            }

            if (string.IsNullOrEmpty(orderDto.PhoneNumber))
            {
                response.Success = false;
                response.Message = "Please provide a valid phone number";
                return response;
            }

            var user = _context.Users.Find(userId);
            if (user == null)
            {
                response.Success = false;
                response.Message = "Unable to create the order";
                return response;
            }

            // Get the user's cart from cache
            var cartDto = await _cartService.GetCart();

            // Check if the cart is empty
            if (cartDto == null || cartDto.CartItems == null || cartDto.CartItems.Count == 0)
            {
                response.Success = false;
                response.Message = "Cannot create order because the cart is empty.";
                return response;
            }

            // Create the order
            Order order = new Order();
            order.UserId = userId;
            order.CreatedAt = DateTime.Now;
            order.ShippingFee = OrderHelper.ShippingFee;
            order.DeliveryAddress = orderDto.DeliveryAddress;
            order.PhoneNumber = orderDto.PhoneNumber;
            order.StripeSessionId = orderDto.StripeSessionId;
            order.PaymentMethod = orderDto.PaymentMethod;
            order.PaymentStatus = orderDto.PaymentStatus; //pending
            order.OrderStatus = orderDto.OrderStatus; //created

            decimal subTotal = 0;
            foreach (var cartItem in cartDto.CartItems)
            {
                int spaproductId = cartItem.SpaProduct.Id;
                int orderedQuantity = cartItem.Quantity;

                var product = _context.SpaProducts.Find(spaproductId);
                if (product == null)
                {
                    response.Success = false;
                    response.Message = "Product with ID " + spaproductId + " is not available";
                    return response;
                }

                // Check if there is enough quantity in stock for the product
                if (product.QuantityInStock < orderedQuantity)
                {
                    response.Success = false;
                    response.Message = "Insufficient quantity in stock for Product ID " + spaproductId;
                    return response;
                }

                var orderItem = new OrderItem();
                orderItem.SpaProductId = spaproductId;
                orderItem.Quantity = orderedQuantity;
                orderItem.UnitPrice = product.Price;

                // Update the quantity in stock for the product
                product.QuantityInStock -= orderedQuantity;
                subTotal += product.Price * orderedQuantity;
                order.OrderItems.Add(orderItem);
            }

            if (order.OrderItems.Count < 1)
            {
                response.Success = false;
                response.Message = "Unable to create order";
                return response;
            }

            // Save the order in the database
            _context.Orders.Add(order);
            _context.SaveChanges();

            // Clear the user's cart from cache
            _cache.Remove("cart");

            // Get rid of the object cycle
            foreach (var item in order.OrderItems)
            {
                item.Order = null;
            }

            order.SubTotal = subTotal;
            order.ShippingFee = OrderHelper.ShippingFee;
            order.TotalPrice = subTotal + OrderHelper.ShippingFee;

            // Hide the user password
            order.User.PasswordHash = null;
            order.User.PasswordSalt = null;

            response.Data = order;
            response.Message = "Order created successfully.";
            return response;
        }


        public async Task<ServiceResponse<List<Order>>> GetOrders(int userId, int pageIndex,int pageSize, string searchPhoneNumber = "", DateTime? fromDate = null, DateTime? toDate = null, string searchStripeSessionId = "", string searchPaymentMethod = "")
        {
            
            var response = new ServiceResponse<List<Order>>();

            // Fetch orders based on the user's role
            string role = _context.Users.Find(userId)?.Role.ToString() ?? "";

                IQueryable<Order> query = _context.Orders.Include(o => o.User)
                    .Include(o => o.OrderItems).ThenInclude(oi => oi.SpaProduct);


                if (role != "Admin")
                {
                    query = query.Where(o => o.UserId == userId);
                }
                

                // Apply search by PhoneNumber if provided
                if (!string.IsNullOrEmpty(searchPhoneNumber))
                {
                    query = query.Where(o => o.PhoneNumber.Contains(searchPhoneNumber));
                }

                // Apply search by StripeSessionId if provided
                if (!string.IsNullOrEmpty(searchStripeSessionId))
                {
                    query = query.Where(o => o.StripeSessionId == searchStripeSessionId);
                }

                // Apply search by PaymentMethod if provided
                if (!string.IsNullOrEmpty(searchPaymentMethod))
                {
                    query = query.Where(o => o.PaymentMethod == searchPaymentMethod);
                }

                // Apply search by Date if provided
                if (fromDate.HasValue && toDate.HasValue)
                    {
                        query = query.Where(o => o.CreatedAt.Date >= fromDate.Value.Date && o.CreatedAt.Date <= toDate.Value.Date);
                    }
                    else if (fromDate.HasValue)
                    {
                        query = query.Where(o => o.CreatedAt.Date >= fromDate.Value.Date);
                    }
                    else if (toDate.HasValue)
                    {                 
                        query = query.Where(o => o.CreatedAt.Date <= toDate.Value.Date);
                    }

                query = query.OrderByDescending(o => o.Id);

                // Read the orders
                var orders = await query.ToListAsync();

                foreach (var order in orders)
                {
                    foreach (var item in order.OrderItems)
                    {
                        item.Order = null;
                    }
                    order.User.PasswordHash = null;
                    order.User.PasswordSalt = null;

                    // Calculate SubTotal and TotalPrice for the order
                    decimal subTotal = 0;
                    foreach (var orderItem in order.OrderItems)
                    {
                        subTotal += orderItem.UnitPrice * orderItem.Quantity;
                    }
                    order.SubTotal = subTotal;
                    order.TotalPrice = subTotal + order.ShippingFee;
                }

                // Perform pagination and get the orders for the current page
                int totalOrders = orders.Count;
                int totalPages = (int)Math.Ceiling((double)totalOrders / pageSize);
                pageIndex = Math.Max(0, Math.Min(pageIndex, totalPages - 1)); // Ensure valid page index

                var pagedOrders = orders.Skip(pageIndex * pageSize).Take(pageSize).ToList();

                response.Data = pagedOrders; // Return the list of orders for the current page
                response.Message = "Order(s) retrieved successfully.";
                var pageInfo = new PageInformation
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalCount = totalOrders,
                    TotalPages = totalPages
                };

                // Set the PageInformation in the response before modifying the pagedOrders list
                response.PageInformation = pageInfo;
                response.Message = "Order(s) retrieved successfully.";

                return response; // Return the ServiceResponse<List<Order>> containing both orders and page info
            }

        public async Task<ServiceResponse<Order>> GetSingleOrder(int orderId, int userId)
        {
            var response = new ServiceResponse<Order>();

            // Fetch the order by its ID
            var order = await _context.Orders.Include(o => o.User)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.SpaProduct)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            // If the order is not found, return an error message
            if (order == null)
            {
                response.Message = "Order not found.";
                return response;
            }

            // Check if the user is an admin or the order belongs to the user
            bool isAdmin = _context.Users.Find(userId)?.Role.ToString() == "Admin";
            if (!isAdmin && order.UserId != userId)
            {
                response.Message = "Access denied. You can only access your own orders.";
                return response;
            }

            // Get rid of the object cycle and hide the user password
            foreach (var item in order.OrderItems)
            {
                item.Order = null;
            }
            order.User.PasswordHash = null;
            order.User.PasswordSalt = null;

            decimal subTotal = 0;
            foreach (var orderItem in order.OrderItems)
            {
                subTotal += orderItem.UnitPrice * orderItem.Quantity;
            }
            order.SubTotal = subTotal;
            order.TotalPrice = subTotal + order.ShippingFee;

            response.Data = order; // Return the order
            response.Message = "Order retrieved successfully.";

            return response; // Return the ServiceResponse<Order> containing the order
        }

        public async Task<ServiceResponse<Order>> UpdateOrder(int id, string? paymentStatus, string? orderStatus, string? deliveryAddress, string? phoneNumber)
        {
            var response = new ServiceResponse<Order>();

            var order = _context.Orders.Find(id);
            if (order == null)
            {
                response.Success = false;
                response.Message = "Order not found.";
                return response;
            }

            if (paymentStatus != null && !OrderHelper.PaymentStatuses.Contains(paymentStatus))
            {
                response.Success = false;
                response.Message = "The Payment Status is not valid";
                return response;
            }

            if (orderStatus != null && !OrderHelper.OrderStatuses.Contains(orderStatus))
            {
                response.Success = false;
                response.Message = "The Order Status is not valid";
                return response;
            }

            if (!string.IsNullOrEmpty(deliveryAddress))
            {
                order.DeliveryAddress = deliveryAddress;
            }

            if (!string.IsNullOrEmpty(phoneNumber))
            {
                order.PhoneNumber = phoneNumber;
            }

            if (paymentStatus != null)
            {
                order.PaymentStatus = paymentStatus;

                if (paymentStatus == "Accepted")
                {
                    // Check and set Order Status
                    if (orderStatus == "Shipped" || orderStatus == "Created" || orderStatus == "Delivered" || orderStatus == "Returned")
                    {
                        order.OrderStatus = orderStatus;
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "Invalid combination of Payment Status and Order Status.";
                        return response;
                    }
                }if (paymentStatus == "Refunded")
                {
                    // Check and set Order Status
                    if (orderStatus == "Returned")
                    {
                        order.OrderStatus = orderStatus;
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "Invalid combination of Payment Status and Order Status.";
                        return response;
                    }
                }
            }

            if (orderStatus != null)
            {
                order.OrderStatus = orderStatus;
            }

            _context.SaveChanges();

            response.Data = order;
            response.Message = "Order updated successfully.";
            return response;
        }



        public async Task<ServiceResponse<Order>> UpdateOrderByCus(int userId, int id, string? deliveryAddress, string? phoneNumber)
        {
            var response = new ServiceResponse<Order>();

            // Fetch the order from the database
            var order = await _context.Orders.Include(o => o.User).FirstOrDefaultAsync(o => o.Id == id);

            // If the order is not found, return an error message
            if (order == null)
            {
                response.Message = "Order not found.";
                return response;
            }

            // Check if the user is the owner of the order
            if (order.UserId != userId)
            {
                response.Message = "Access denied. You can only update your own orders.";
                return response;
            }

            // Customers can update PhoneNumber and DeliveryAddress only when the OrderStatus is "Created"
            if (order.OrderStatus != "Created")
            {
                response.Message = "Access denied. You can only update order details for orders with 'Created' status.";
                return response;
            }

            // Update the order details based on the input
            if (!string.IsNullOrEmpty(deliveryAddress))
            {
                order.DeliveryAddress = deliveryAddress;
            }

            if (!string.IsNullOrEmpty(phoneNumber))
            {
                order.PhoneNumber = phoneNumber;
            }

            // Save the updated order to the database
            _context.SaveChanges();

            // Get rid of the object cycle and hide the user password
            foreach (var item in order.OrderItems)
            {
                item.Order = null;
            }
            order.User.PasswordHash = null;
            order.User.PasswordSalt = null;

            response.Data = order; // Return the updated order
            response.Message = "Order updated successfully.";

            return response; // Return the ServiceResponse<Order> containing the updated order
        }
    }
}