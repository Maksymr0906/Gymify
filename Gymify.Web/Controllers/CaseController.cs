using Gymify.Application.DTOs.Case;
using Gymify.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gymify.Web.Controllers;

[Authorize]
public class CaseController : Controller
{
    private readonly ICaseService _caseService;

    public CaseController(ICaseService caseService)
    {
        _caseService = caseService;
    }

    // GET: показати сторінку кейсу тут тіпа ім'я його картінка
    
    [HttpGet]
    public async Task<IActionResult> Details()
    {
        var caseId = Guid.Parse("20000000-0000-0000-0000-000000000045");

        var caseInfoDto = await _caseService.GetCaseDetailsAsync(caseId);

        return View(caseInfoDto); // модель для Razor
    }

    // POST: відкриття кейсу, в параметр кидаємо з сесії гуйд юзера
    [HttpPost]
	public async Task<IActionResult> OpenCase()
	{
        var caseId = Guid.Parse("20000000-0000-0000-0000-000000000045");

        var user = User.FindFirst("UserProfileId") ?? throw new Exception("User not found");
		var userId = Guid.Parse(user.Value);

		var result = await _caseService.OpenCaseAsync(userId, caseId);

		return Json(result);
	}

}
