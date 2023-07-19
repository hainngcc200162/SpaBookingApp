using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpaBookingApp.Dtos.Provision;
using SpaBookingApp.Services.ProvisionService;
using System.Threading.Tasks;

namespace SpaBookingApp.Pages.Provisions
{
    public class DeleteProvisionModel : PageModel
    {
        private readonly IProvisionService _provisionService;

        public DeleteProvisionModel(IProvisionService provisionService)
        {
            _provisionService = provisionService;
        }

        [BindProperty]
        public GetProvisionDto Provision { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var serviceResponse = await _provisionService.GetProvisionById(id);
            if (serviceResponse.Success)
            {
                Provision = serviceResponse.Data;
                return Page();
            }
            else
            {
                ErrorMessage = serviceResponse.Message;
                return RedirectToPage("/Provisions/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var serviceResponse = await _provisionService.DeleteProvision(id);
            if (serviceResponse.Success)
            {
                return RedirectToPage("/Provisions/Index");
            }
            else
            {
                ErrorMessage = serviceResponse.Message;
                return RedirectToPage("/Provisions/Index");
            }
        }
    }
}
