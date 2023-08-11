using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Dtos.SpaProduct
{
    public class UpdateSpaProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "The Ordinary";
        public decimal Price { get; set; } = 10;
        public int QuantityInStock { get; set; } = 45;
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public IFormFile Poster { get; set; }
        public string PosterName { get; set; }
        
    }
}