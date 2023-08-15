using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace SpaBookingApp.Pages.Staffs
{
    public class GetStaffById : PageModel
    {
        private readonly ILogger<GetStaffById> _logger;

        public GetStaffById(ILogger<GetStaffById> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}