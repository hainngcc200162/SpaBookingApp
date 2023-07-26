using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


namespace SpaBookingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly DataContext _context;

        public CartController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("PaymentMethods")]
        public IActionResult GetPayMethods()
        {
            return Ok(OrderHelper.PaymentMethods);
        }

        [HttpGet]
        public IActionResult GetCart(string productIdentifiers)
        {
            CartDto cartDto = new CartDto();
            cartDto.CartItems = new List<CartItemDto>();
            cartDto.SubTotal = 0;
            cartDto.ShippingFee = OrderHelper.ShippingFee;
            cartDto.TotalPrice = 0;

            var productDictionary = OrderHelper.GetProductDictionary(productIdentifiers);

            foreach (var pair in productDictionary)
            {
                int productId = pair.Key;
                var product = _context.Products.Find(productId);
                if (product == null)
                {
                    continue;
                }

                var cartItemDto = new CartItemDto();
                cartItemDto.Product = product;
                cartItemDto.Quantity = pair.Value;

                cartDto.CartItems.Add(cartItemDto);
                cartDto.SubTotal += product.Price * pair.Value;
                cartDto.TotalPrice = cartDto.SubTotal + cartDto.ShippingFee;
            }

            return Ok(cartDto);
        }
    }
}