using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Dtos.User
{
    public class UserLoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; } 
        public bool IsVerified { get; set; } = true ;// Add IsVerified property
        
    }
}