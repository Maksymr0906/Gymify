using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gymify.Pages
{
    public class WorkoutModel : PageModel
    {
        private readonly ILogger<WorkoutModel> _logger;

        public WorkoutModel(ILogger<WorkoutModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }

}
