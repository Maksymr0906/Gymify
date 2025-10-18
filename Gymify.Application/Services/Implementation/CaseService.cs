using Gymify.Application.Services.Interfaces;
using Gymify.Data.Entities;
using Gymify.Data.Enums;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Application.Services.Implementation;

public class CaseService(IUnitOfWork unitOfWork) : ICaseService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly Random _random = new Random();

    public async Task<List<UserCase>> GenerateRewardsAsync(Guid userProfileId, List<Achievement> newAchievements, bool isLevelUp)
    {
        throw new NotImplementedException();

        //var userProfile = await _unitOfWork.UserProfileRepository.GetByIdAsync(userProfileId);
        //if (userProfile == null)
        //    throw new Exception("UserProfile not found");

        //var issuedCases = new List<UserCase>();

        //foreach (var achievement in newAchievements)
        //{
        //    if (achievement.RewardCaseType.HasValue)
        //    {
        //        var caseToGive = await _unitOfWork.CaseRepository
        //            .GetFirstOrDefaultAsync(c => c.Type == achievement.RewardCaseType.Value);

        //        if (caseToGive != null)
        //        {
        //            var userCase = new UserCase
        //            {
        //                UserProfileId = userProfile.Id,
        //                CaseId = caseToGive.Id
        //            };
        //            await _unitOfWork.UserCaseRepository.CreateAsync(userCase);
        //            issuedCases.Add(userCase);
        //        }
        //    }
        //}

        //if (isLevelUp)
        //{
        //    var allCases = await _unitOfWork.CaseRepository.GetAllAsync();
        //    var eligibleCases = allCases.Where(c => c.Type != CaseType.Common).ToList();
        //    if (eligibleCases.Any())
        //    {
        //        var randomCase = eligibleCases[_random.Next(eligibleCases.Count)];

        //        var userCase = new UserCase
        //        {
        //            UserProfileId = userProfile.Id,
        //            CaseId = randomCase.Id
        //        };

        //        await _unitOfWork.UserCaseRepository.CreateAsync(userCase);
        //        issuedCases.Add(userCase);
        //    }
        //}

        //await _unitOfWork.SaveAsync();
        //return issuedCases;
    }
}
