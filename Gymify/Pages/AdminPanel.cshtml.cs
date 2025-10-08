using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gymify.Pages
{
    public class AdminPanelModel : PageModel
    {
        private readonly ILogger<AdminPanelModel> _logger;

        public AdminPanelModel(ILogger<AdminPanelModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }

}
