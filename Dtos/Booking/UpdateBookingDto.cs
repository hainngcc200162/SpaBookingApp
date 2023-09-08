using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Dtos.Booking
{
    public class UpdateBookingDto
    {
        
        public int DepartmentId { get; set; }
        public int StaffId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; } = DateTime.MinValue;
        public string Status { get; set; }
        public string Note { get; set; }
        public List<int> ProvisionIds { get; set; }
        public List<ProvisionRemainingExecutionDto> ProvisionRemainingExecutions { get; set; }

    }
}