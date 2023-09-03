using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Dtos.Booking
{
    public class AddBookingDto
    {
        public List<int> ProvisionIds { get; set; }

        // Các trường khác của Booking
        public int DepartmentId { get; set; }
        public int StaffId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; } = DateTime.MinValue;
        public string Status { get; set; } = "Waiting";
        public string Note { get; set; } = string.Empty;

    }
}