using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Models
{
    public class OrderDto
    {

        [Required, MinLength(30), MaxLength(100)]
        public string DeliveryAddress { get; set; } = "";

        [Required, MaxLength(20)]
        public string PhoneNumber { get; set; } = "";

        [Required]
        public string PaymentMethod { get; set; } = "";

        [MaxLength(30)]
        public string OrderStatus { get; set; } = "Created";

        [MaxLength(30)]
        public string PaymentStatus { get; set; } = "Pending";

        public string StripeSessionId { get; set; } = "";
    }
}