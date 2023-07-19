using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Dtos.Provision
{
    public class AddProvisionDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool Status { get; set; }
        public IFormFile Poster { get; set; }       
    }
}