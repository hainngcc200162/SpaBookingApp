using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Models
{
    public class CartItemDto
    {
        public SpaProduct SpaProduct { get; set; } = new SpaProduct();
        public int Quantity { get; set; }
    }
}