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
        public async Task<IActionResult> GetCart()
        {
            var cartDto = await _cartService.GetCart();
            return Ok(cartDto);
        }

        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart(string productIdentifiers)
        {
            var cartDto = await _cartService.AddToCart(productIdentifiers);
            return Ok(cartDto);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCart(string productIdentifiers)
        {
            var cartDto = await _cartService.UpdateCart(productIdentifiers);
            return Ok(cartDto);
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var cartDto = await _cartService.ClearCart();
            return Ok(cartDto);
        }
    }
}