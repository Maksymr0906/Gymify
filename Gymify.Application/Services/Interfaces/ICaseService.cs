using Gymify.Application.DTOs.Case;
using Gymify.Data.Entities;

namespace Gymify.Application.Services.Interfaces;

public interface ICaseService
{
    Task GiveRewardByLevelUp(Guid userProfileId, int levelsUp);
    Task GiveRewardByAchievement(Guid userProfileId, Guid rewardItemId);
    Task<CaseInfoDto> GetCaseDetailsAsync(Guid caseId, bool ukranianVer);
    Task<ICollection<CaseInfoDto>> GetAllUserCasesAsync(Guid userProfileId, bool ukranianVer);
    Task<OpenCaseResultDto> OpenCaseAsync(Guid userId, Guid caseId, bool ukranianVer);
}
