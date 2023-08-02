using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Dtos.Subject
{
    public class GetSubjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

    }
}