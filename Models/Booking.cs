using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int DepartmentId { get; set; }
        public int StaffId { get; set; }
        public DateTime StartTime { get; set; } 
        public DateTime EndTime { get; set; }
        public string Status { get; set; } = "Waiting";
        public User User { get; set; } 
        public Staff Staff { get; set; }
        public Department Department { get; set; }
        public string Note { get; set; } = string.Empty;

        public List<ProvisionBooking> ProvisionBookings { get; set; } = new List<ProvisionBooking>();
        
    }
}