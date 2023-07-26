using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Models
{
    public class CartDto
    {
        public List<CartItemDto> CartItems { get; set; } = new();
        public decimal SubTotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalPrice { get; set; }

    }
}