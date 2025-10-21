using Gymify.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gymify.Web.Controllers;

public class CaseController : Controller
{
    private readonly ICaseService _caseService;

    public CaseController(ICaseService caseService)
    {
        _caseService = caseService;
    }

    // GET: показати сторінку кейсу тут тіпа ім'я його картінка
    [HttpGet]
    public async Task<IActionResult> Details(Guid caseId)
    {
        var caseInfo = await _caseService.GetCaseDetailsAsync(caseId);
        return View(caseInfo); // модель для Razor
    }

    // POST: відкриття кейсу
    [HttpPost]
    public async Task<IActionResult> Open(Guid userId, Guid caseId)
    {
        var result = await _caseService.OpenCaseAsync(userId, caseId);
        return Json(result);
    }
}
