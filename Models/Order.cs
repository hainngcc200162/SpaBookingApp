using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }

        [Precision(16, 2)]
        public decimal ShippingFee { get; set; }

        [MaxLength(100)]
        public string DeliveryAddress { get; set; } = "";

        [MaxLength(20)]
        public string PhoneNumber { get; set; } = "";

        [MaxLength(30)]
        public string PaymentMethod { get; set; } = "";

        [MaxLength(30)]
        public string PaymentStatus { get; set; } = "";

        [MaxLength(30)]
        public string OrderStatus { get; set; } = "";


        // navigation properties
        public User User { get; set; } = null!;

        public List<OrderItem> OrderItems { get; set; } = new();
    }
}