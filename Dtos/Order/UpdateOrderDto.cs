using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Dtos.Order
{
    public class UpdateOrderDto
    {
        [Required, MinLength(30), MaxLength(100)]
        public string DeliveryAddress { get; set; } = "";

        [Required, MaxLength(20)]
        public string PhoneNumber { get; set; } = "";

        [Required]
        public string OrderStatus { get; set; } = "";

        [Required]
        public string PaymentStatus { get; set; } = "";
    }
}