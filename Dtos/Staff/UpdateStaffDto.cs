using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Dtos.Staff
{
    public class UpdateStaffDto
    {   
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public StaffGender Gender { get; set; }
        public string Gmail { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}