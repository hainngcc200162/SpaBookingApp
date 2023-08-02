using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Message { get; set; }
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}