using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Models
{
    public class OrderDto
    {
        [Required]
        public string ProductIdentifiers { get; set; } = "";

        [Required, MinLength(30), MaxLength(100)]
        public string DeliveryAddress { get; set; } = "";

        [Required]
        public string PaymentMethod { get; set; } = "";
    }
}