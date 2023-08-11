using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Dtos.SpaProduct
{
    public class GetSpaProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int QuantityInStock { get; set; }
        public string CategoryName { get; set; }
        public string CategoryId { get; set; }
        public string Description { get; set; }
        public string PosterName { get; set; }
        
    }
}