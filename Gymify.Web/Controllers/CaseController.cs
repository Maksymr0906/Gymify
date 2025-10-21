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
    public IActionResult Details(CaseInfoDto caseInfoDto)
    {
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
    public async Task<IActionResult> OpenCase(Guid userId)
    {
        _caseViewModel ??= new CaseViewModel();
        var result = await _caseService.OpenCaseAsync(userId, _caseViewModel.CaseInfo.CaseId);

        _caseViewModel.OpenCaseResult = result;

        return Json(result);
    }
}
