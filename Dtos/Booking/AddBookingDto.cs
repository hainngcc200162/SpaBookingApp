using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Dtos.Booking
{
    public class AddBookingDto
    {
        [Required(ErrorMessage = "ProvisionIds is required.")]
        public List<int> ProvisionIds { get; set; }

        [Required(ErrorMessage = "DepartmentId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "DepartmentId must be greater than zero.")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "StaffId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "StaffId must be greater than zero.")]
        public int StaffId { get; set; }

        [Required(ErrorMessage = "StartTime is required.")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "EndTime is required.")]
        public DateTime EndTime { get; set; } = DateTime.MinValue;

        public string Status { get; set; } = "Waiting";

        public string Note { get; set; } = string.Empty;
    }
}