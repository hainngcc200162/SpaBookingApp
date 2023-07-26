using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Models
{
    public class PasswordReset
    {
        public int Id { get; set; }
        public string Email { get; set; } = "";
        public string Token { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}