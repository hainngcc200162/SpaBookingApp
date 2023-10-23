using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Models
{
    public class Provision
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
        public bool Status { get; set; }
        public string PosterName { get; set; }
        public int NumberOfExecutions { get; set; }
        public bool IsDeleted { get; set; } = false;
        
       public List<ProvisionBooking> ProvisionBookings { get; set; } = new List<ProvisionBooking>();

    }
}