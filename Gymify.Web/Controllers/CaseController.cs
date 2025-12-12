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

    [HttpPost]
    public async Task<IActionResult> OpenCase(Guid caseId)
    {
        try
        {
            var user = User.FindFirst("UserProfileId") ?? throw new Exception("User not found");
            var userId = Guid.Parse(user.Value);
            var result = await _caseService.OpenCaseAsync(userId, caseId, IsUkrainian);

            return Json(result);
        }
        catch (Exception ex) 
        {
            if (ex.Message.Contains("No userCase found"))
            {
                TempData["QuickError"] = IsUkrainian ? "Нажаль, кейси цього типу закінчилися." : "Unfortunately, you don't have cases of this type.";

                return Json(new
                {
                    redirectUrl = Url.Action("Index", "Inventory")
                });
            }

            return StatusCode(500, new { message = ex.Message });
        }
    }

}
