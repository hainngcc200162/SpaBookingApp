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
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("PaymentMethods")]
        public IActionResult GetPayMethods()
        {
            return Ok(OrderHelper.PaymentMethods);
        }
        [HttpGet]
        public async Task<IActionResult> GetCart(string productIdentifiers)
        {
            var cartDto = await _cartService.GetCart(productIdentifiers);
            return Ok(cartDto);
        }
    }
}