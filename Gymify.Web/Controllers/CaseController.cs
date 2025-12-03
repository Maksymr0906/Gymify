using Gymify.Application.DTOs.Case;
using Gymify.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Gymify.Web.Controllers;

[Authorize]
public class CaseController : Controller
{
    private readonly ICaseService _caseService;
    private bool IsUkrainian => CultureInfo.CurrentCulture.Name == "uk-UA" || CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "uk";

    public CaseController(ICaseService caseService)
    {
        _caseService = caseService;
    }

    // GET: показати сторінку кейсу тут тіпа ім'я його картінка
    
    [HttpGet]
    public async Task<IActionResult> Details(Guid caseId)
    {

        var caseInfoDto = await _caseService.GetCaseDetailsAsync(caseId, IsUkrainian);

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
