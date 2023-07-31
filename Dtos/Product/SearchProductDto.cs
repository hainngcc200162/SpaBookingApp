using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Dtos.Product
{
    public class SearchProductDto
    {
        public string Search { get; set; }
        public string CategoryName { get; set; }
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        public string Sort { get; set; }
        public string Order { get; set; }
        public int? Page { get; set; }
    }
}