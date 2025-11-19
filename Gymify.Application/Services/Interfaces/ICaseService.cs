using Gymify.Application.DTOs.Case;
using Gymify.Data.Entities;

namespace Gymify.Application.Services.Interfaces;

public interface ICaseService
{
    Task GiveRewardByLevelUp(Guid userProfileId, int levelsUp);
    Task GiveRewardByAchievement(Guid userProfileId, Guid rewardItemId);
    Task<CaseInfoDto> GetCaseDetailsAsync(Guid caseId);
    Task<ICollection<CaseInfoDto>> GetAllUserCasesAsync(Guid userProfileId);
    Task<OpenCaseResultDto> OpenCaseAsync(Guid userId, Guid caseId);
}
