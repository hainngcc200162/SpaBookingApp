    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    namespace SpaBookingApp.Models
    {
        public class SpaProduct
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
            public int QuantityInStock { get; set; }
            public int CategoryId { get; set; }
            public Category Category { get; set; }
            public string Description { get; set; }
            public string PosterName { get; set; }
            public bool IsDeleted { get; set; } = false;

        }
    }
