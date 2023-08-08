using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string OpeningHours { get; set; } = string.Empty;

        //link to booking
        public List<Booking> Bookings { get; set; } = new List<Booking>();
    }
}