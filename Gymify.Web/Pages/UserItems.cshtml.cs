using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gymify.Pages;

public class UserItemsModel : PageModel
{
    private readonly ILogger<UserItemsModel> _logger;

    public UserItemsModel(ILogger<UserItemsModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
    }
}