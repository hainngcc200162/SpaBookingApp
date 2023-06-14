using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Dtos.User
{
    public class UserChangePasswordDto
    {
        public string Username { get; set; } = string.Empty;
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    
    }
}