using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gymify.Pages
{
    public class ChangePasswordModel : PageModel
    {
        private readonly ILogger<ChangePasswordModel> _logger;

        public ChangePasswordModel(ILogger<ChangePasswordModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }

}
