using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Dtos.Booking
{
    public class GetBookingDto
    {   
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserEmail { get; set; }
        public string UserPhoneNumber { get; set; }
        public int ProvisionId { get; set; }
        public string ProvisionName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int StaffId { get; set; }
        public string StaffName { get; set; }
        public DateTime StartTime { get; set; } 
        public DateTime EndTime { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
    }
}