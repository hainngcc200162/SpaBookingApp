using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Dtos.Booking
{
    public class UpdateBookingDto
    {
        
        public int Id { get; set; }
        public int ProvisionId { get; set; }
        public int AppartmentId { get; set; }
        public int StaffId { get; set; }
        public DateTime StartTime { get; set; } 
        public DateTime EndTime { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
    }
}