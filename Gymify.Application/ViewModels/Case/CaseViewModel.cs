using Gymify.Application.DTOs.Case;

namespace Gymify.Application.ViewModels.Case;
public class CaseViewModel
{
    // Інформація про сам кейс
    public CaseInfoDto CaseInfo { get; set; } = new CaseInfoDto();

    // Результат відкриття кейсу
    public OpenCaseResultDto OpenCaseResult { get; set; } = new OpenCaseResultDto();
}

