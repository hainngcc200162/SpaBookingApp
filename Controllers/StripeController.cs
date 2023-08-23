using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using SpaBookingApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SpaBookingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StripeController : ControllerBase
    {
        private readonly StripeSettings _stripeSettings;

        public StripeController(IOptions<StripeSettings> stripeSettings)
        {
            _stripeSettings = stripeSettings.Value;
        }

        [HttpGet]
        [Route("index")] // Route cho action Index
        public IActionResult Index()
        {
            return Ok("Welcome to the API!");
        }

        [HttpPost]
        [Route("checkout")]
        public IActionResult CreateCheckoutSession([FromBody] string randomString)
        {
            var currency = "usd";
            var successUrl = "http://localhost:5119/successCheckout/success";
            var cancelUrl = "https://localhost:7196/api/stripe/cancel";
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;

            var totalPrice = HttpContext.Session.Get<decimal>("TotalPrice");

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
        {
            "card"
        },
                LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = currency,
                    UnitAmount = Convert.ToInt32(totalPrice * 100),
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Product Name",
                        Description = "Payment ID: " + randomString // Sử dụng chuỗi ngẫu nhiên từ request data
                    }
                },
                Quantity = 1
            }
        },
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl
            };

            var service = new SessionService();
            var session = service.Create(options);

            return Ok(new { SessionUrl = session.Url });
        }



        [HttpGet]
        [Route("success")] // Route cho action success
        public IActionResult Success()
        {
            return Ok("Payment successful!");
        }

        [HttpGet]
        [Route("cancel")] // Route cho action cancel
        public IActionResult Cancel()
        {
            return Ok("Payment canceled!");
        }

        [HttpGet]
        [Route("error")] // Route cho action error
        public IActionResult Error()
        {
            return BadRequest("An error occurred!");
        }
    }
}
