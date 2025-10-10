using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gymify.Pages;

public class CreateWorkoutModel : PageModel
{
    private readonly ILogger<CreateWorkoutModel> _logger;

    public CreateWorkoutModel(ILogger<CreateWorkoutModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
    }
}