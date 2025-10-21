using Gymify.Application.DTOs.Case;
using Gymify.Application.Services.Interfaces;
using Gymify.Application.ViewModels.Case;
using Microsoft.AspNetCore.Mvc;

namespace Gymify.Web.Controllers;

public class CaseController : Controller
{
    private readonly ICaseService _caseService;
    private CaseViewModel? _caseViewModel;

    public CaseController(ICaseService caseService)
    {
        _caseService = caseService;
    }

    // GET: показати сторінку кейсу тут тіпа ім'я його картінка
    
    [HttpGet]
    public async Task<IActionResult> Details(Guid caseId)
    {
        var caseInfoDto = await _caseService.GetCaseDetailsAsync(caseId);

        var viewModel = new CaseViewModel()
        {
            CaseInfo = caseInfoDto,
            OpenCaseResult = new OpenCaseResultDto()
        };

        _caseViewModel = viewModel;

        return View(viewModel); // модель для Razor
    }

    // POST: відкриття кейсу, в параметр кидаємо з сесії гуйд юзера
    [HttpPost]
    public async Task<IActionResult> OpenCase()
    {
		var userId = Guid.Parse(User.FindFirst("UserProfileId").Value);

		if (userId == null)
			throw new Exception("User not authorized");

		if (_caseViewModel.CaseInfo == null || _caseViewModel.OpenCaseResult == null || _caseViewModel == null)
			throw new Exception("CaseViewModel is not complete, something is missing");

		var result = await _caseService.OpenCaseAsync(userId, _caseViewModel.CaseInfo.CaseId);

        _caseViewModel.OpenCaseResult = result;

        return Json(result);
    }
}
