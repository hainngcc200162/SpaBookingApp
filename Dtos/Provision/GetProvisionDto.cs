using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Dtos.Provision
{
    public class GetProvisionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
        public int NumberOfExecutions { get; set; }
        public bool Status { get; set; }
        public string PosterName { get; set; }
        // Thêm thuộc tính RemainingExecutions
        public int RemainingExecutions { get; set; }
    }
}