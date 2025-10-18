using Gymify.Application.Services.Interfaces;
using Gymify.Data.Entities;
using Gymify.Data.Enums;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Application.Services.Implementation;

public class CaseService(IUnitOfWork unitOfWork) : ICaseService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly Random _random = new Random();

    public async Task GenerateRewardsAsync(
    Guid userProfileId,
    List<Achievement> newAchievements,
    bool isLevelUp)
    {
        var userProfile = await _unitOfWork.UserProfileRepository.GetByIdAsync(userProfileId);
        if (userProfile == null)
            throw new Exception("UserProfile not found");

        
        foreach (var achievement in newAchievements)
        {
            var caseToGive = await _unitOfWork.CaseRepository
                    .GetByCaseTypeAsync(achievement.RewardCaseType);

            if (caseToGive != null)
            {
                var userCase = new UserCase
                {
                    UserProfileId = userProfile.Id,
                    CaseId = caseToGive.Id
                };

                await _unitOfWork.UserCaseRepository.CreateAsync(userCase);
            }
        }

        if (isLevelUp)
        {
            var allCases = (await _unitOfWork.CaseRepository.GetAllAsync()).ToList();
            if (allCases.Any())
            {
                var randomCase = allCases[_random.Next(allCases.Count)];

                var userCase = new UserCase
                {
                    UserProfileId = userProfile.Id,
                    CaseId = randomCase.Id
                };

                await _unitOfWork.UserCaseRepository.CreateAsync(userCase);
            }
        }


        await _unitOfWork.SaveAsync();
    }

}
