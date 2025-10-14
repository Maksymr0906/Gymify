using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gymify.Web.Pages;

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
