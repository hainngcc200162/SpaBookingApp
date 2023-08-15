using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Models
{
    public class ProvisionBooking
    {
        public int Id { get; set; }

        public int ProvisionId { get; set; }
        public Provision Provision { get; set; }

        public int BookingId { get; set; }
        public Booking Booking { get; set; }
    }

}