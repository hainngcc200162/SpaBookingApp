using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int SpaProductId { get; set; }

        public int Quantity { get; set; }

        [Precision(16, 2)]
        public decimal UnitPrice { get; set; }


        // navigation properties
        public Order Order { get; set; } = null!;

        public SpaProduct SpaProduct { get; set; } = null!;
    }
}