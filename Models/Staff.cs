using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Models
{
    public class Staff
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public StaffGender Gender { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PosterName { get; set; }
        public bool IsDeleted { get; set; } = false;
        //
        public List<Booking> Bookings { get; set; } = new List<Booking>();
    }
}