using Gymify.Application.DTOs.Case;
using Gymify.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Gymify.Web.Controllers;

[Authorize]
public class CaseController : BaseController
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
        var caseInfoDto = await _caseService.GetCaseDetailsAsync(caseId, IsUkrainian);

        if (caseInfoDto == null)
        {
            return NotFound();
        }

        return View(caseInfoDto);
    }

    // POST: відкриття кейсу, в параметр кидаємо з сесії гуйд юзера
    [HttpPost]
    public async Task<IActionResult> OpenCase(Guid caseId)
    {
        var user = User.FindFirst("UserProfileId") ?? throw new Exception("User not found");

        var userId = Guid.Parse(user.Value);

        var result = await _caseService.OpenCaseAsync(userId, caseId, IsUkrainian);
        return Json(result);
    }

}
