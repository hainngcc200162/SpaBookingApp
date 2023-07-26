using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Models
{
    public class CartItemDto
    {
        public Product Product { get; set; } = new Product();
        public int Quantity { get; set; }
    }
}