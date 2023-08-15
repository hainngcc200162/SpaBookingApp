using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace SpaBookingApp.Pages.Staffs
{
    public class DeleteStaff : PageModel
    {
        private readonly ILogger<DeleteStaff> _logger;

        public DeleteStaff(ILogger<DeleteStaff> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}