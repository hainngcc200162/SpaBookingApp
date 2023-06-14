using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = "The Ordinary";
        public decimal Price { get; set; } = 10;
        public int QuantityInStock { get; set; } = 45;
        public int CategoryId { get; set; }  
        public Category Category { get; set; }  
    }
}