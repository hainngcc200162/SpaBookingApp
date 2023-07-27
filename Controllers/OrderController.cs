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
        private readonly DataContext _context;
        public OrderController(DataContext context)
        {
            _context = context;
        }
        [Authorize]
        [HttpPost]
        public IActionResult CreateOrder(OrderDto orderDto)
        {
            //check if the payment method is valid or not
            if (!OrderHelper.PaymentMethods.ContainsKey(orderDto.PaymentMethod))
            {
                ModelState.AddModelError("Payment Method", "Please select a valid payment method");
                return BadRequest(ModelState);
            }

            int userId = JwtReader.GetUserId(User);
            Console.WriteLine("userId: " + userId);
            var user = _context.Users.Find(userId);
            if (user == null)
            {
                ModelState.AddModelError("Order", "Unable to create the order1");
                return BadRequest(ModelState);
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
                    ModelState.AddModelError("Product", "Product with ID " + productId + " is not available");
                    return BadRequest(ModelState);
                }
                // Check if there is enough quantity in stock for the product
                if (product.QuantityInStock < orderedQuantity)
                {
                    ModelState.AddModelError("Product", "Insufficient quantity in stock for Product ID " + productId);
                    return BadRequest(ModelState);
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
                ModelState.AddModelError("Order", "Unable to create order");
                return BadRequest(ModelState);
            }

            //save in the database
            _context.Orders.Add(order);
            _context.SaveChanges();

            //get rig of the object cycle
            foreach (var item in order.OrderItems)
            {
                item.Order = null;
            }

            //hide the user password


            return Ok(order);
        }
    }
}