using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace SpaBookingApp.Pages.Staffs
{
    public class UpdateStaff : PageModel
    {
        private readonly ILogger<UpdateStaff> _logger;

        public UpdateStaff(ILogger<UpdateStaff> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}