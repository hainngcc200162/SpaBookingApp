using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Dtos.Category
{
    public class UpdateCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}