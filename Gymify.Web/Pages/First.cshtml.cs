using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gymify.Pages
{
    public class FirstModel : PageModel
    {
        private readonly ILogger<FirstModel> _logger;

        public FirstModel(ILogger<FirstModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }

}
