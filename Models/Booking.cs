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
        public int ProvisionId { get; set; }
        public int AppartmentId { get; set; }
        public int StaffId { get; set; }
        public DateTime StartTime { get; set; } 
        public DateTime EndTime { get; set; }
        public int Status { get; set; }
        public User User { get; set; } 
        public Provision Provision { get; set; }
        public Staff Staff { get; set; }
        public Appartment Appartment { get; set; }
        
    }
}