using Microsoft.AspNetCore.Http;
using SpaBookingApp.Models;

namespace SpaBookingApp.Dtos.Product
{
    public class AddProductDto
    {
        public string Name { get; set; } = "The Ordinary";
        public decimal Price { get; set; } = 10;
        public int QuantityInStock { get; set; } = 45;
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public IFormFile Poster { get; set; }
    }
}
