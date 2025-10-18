﻿using Gymify.Application.DTOs.Workout;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Application.Services.Implementation;

public class WorkoutService(IUnitOfWork unitOfWork, IUserProfileService userProfileService, IAchievementService achievementService, ICaseService caseService)
    : IWorkoutService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUserProfileService _userProfileService = userProfileService;
    private readonly IAchievementService _achievementService = achievementService;
    private readonly ICaseService _caseService = caseService;

    public async Task<WorkoutDto> CompleteWorkoutAsync(CompleteWorkoutRequestDto model)
    {
        var workout = await _unitOfWork.WorkoutRepository.GetByIdWithDetailsAsync(model.WorkoutId);

        if (workout == null)
            throw new Exception("Workout not found");

        workout.Conclusion = model.Conclusions;

        var totalXp = workout.Exercises.Sum(e => e.EarnedXP);
        workout.TotalXP = totalXp;

        await _unitOfWork.WorkoutRepository.UpdateAsync(workout);

        var userProfile = await _unitOfWork.UserProfileRepository.GetByIdAsync(workout.UserProfileId);

        var oldLevel = userProfile.Level;
        await _userProfileService.AddXPAsync(workout.UserProfileId, totalXp);
        await _userProfileService.UpdateStatsAsync(workout.UserProfileId, workout.Id);

        bool isLevelUp = userProfile.Level > oldLevel;

        var newAchievements = await _achievementService.CheckForAchievementsAsync(userProfile.Id);

        var rewards = await _caseService.GenerateRewardsAsync(userProfile.Id, newAchievements, isLevelUp);

        await _unitOfWork.SaveAsync();

        return new WorkoutDto
        {
            Id = workout.Id,
            Name = workout.Name,
            Description = workout.Description,
            Conclusion = workout.Conclusion,
            IsPrivate = workout.IsPrivate,
            TotalXP = workout.TotalXP,
            UserProfileId = workout.UserProfileId
        };
    }

    public async Task<WorkoutDto> CreateWorkoutAsync(CreateWorkoutRequestDto model)
    {
        var workout = new Workout
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Description = model.Description,
            UserProfileId = model.UserProfileId,
        };

        await _unitOfWork.WorkoutRepository.CreateAsync(workout);
        await _unitOfWork.SaveAsync();

        return new WorkoutDto
        {
            Id = workout.Id,
            Name = workout.Name,
            Description = workout.Description,
            Conclusion = workout.Conclusion,
            IsPrivate = workout.IsPrivate,
            TotalXP = workout.TotalXP,
            UserProfileId = workout.UserProfileId,
        };
    }
}
