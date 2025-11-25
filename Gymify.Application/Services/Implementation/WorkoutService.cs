using Gymify.Application.DTOs.Achievement;
using Gymify.Application.DTOs.UserExercise;
using Gymify.Application.DTOs.Workout;
using Gymify.Application.DTOs.WorkoutsFeed;
using Gymify.Application.Services.Interfaces;
using Gymify.Application.ViewModels.Workout;
using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;
using Gymify.Application.ViewModels.Comment;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Application.Services.Implementation;

public class WorkoutService(IUnitOfWork unitOfWork, IUserProfileService userProfileService, IAchievementService achievementService, ICaseService caseService)
    : IWorkoutService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUserProfileService _userProfileService = userProfileService;
    private readonly IAchievementService _achievementService = achievementService;
    private readonly ICaseService _caseService = caseService;

    public async Task<CompleteWorkoutResponseDto> CompleteWorkoutAsync(CompleteWorkoutRequestDto model)
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

        userProfile = await _unitOfWork.UserProfileRepository.GetByIdAsync(workout.UserProfileId);
        var newLevel = userProfile.Level;

        var levelsUp = newLevel - oldLevel;

        await _userProfileService.UpdateStatsAsync(workout.UserProfileId, workout.Id);

        await _caseService.GiveRewardByLevelUp(userProfile.Id, levelsUp);
        
        var newAchievements = await _achievementService.UpdateUserAchievementsAsync(userProfile.Id);

        await _unitOfWork.SaveAsync();

        return new CompleteWorkoutResponseDto
        {
            WorkoutDto = new WorkoutDto
            {
                Id = workout.Id,
                Name = workout.Name,
                Description = workout.Description,
                Conclusion = workout.Conclusion,
                IsPrivate = workout.IsPrivate,
                TotalXP = workout.TotalXP,
                UserProfileId = workout.UserProfileId
            },
            AchievementDtos = newAchievements.Select(a => new AchievementDto
            {
                AchievementId = a.Id,
                Name = a.Name,
                Description = a.Description,
                IconUrl = a.IconUrl,
                RewardItemId = a.RewardItemId,
                TargetProperty = a.TargetProperty,
                TargetValue = a.TargetValue,
                ComparisonType = a.ComparisonType,
                Progress = 0,
                IsCompleted = false,
                UnlockedAt = a.CreatedAt
            }).ToList()
        };
    }

    public async Task<WorkoutDto> CreateWorkoutAsync(CreateWorkoutRequestDto model, Guid userProfileId)
    {
        var workout = new Workout
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Description = model.Description,
            UserProfileId = userProfileId
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

    public async Task<ICollection<WorkoutDto>> GetAllUserWorkoutsAsync(Guid userProfileId)
    {
        var workouts = await _unitOfWork.WorkoutRepository.GetAllByUserIdAsync(userProfileId);

        var workoutDtos = workouts.Select(workout => new WorkoutDto
        {
            Id = workout.Id,
            Name = workout.Name,
            Description = workout.Description,
            Conclusion = workout.Conclusion,
            IsPrivate = workout.IsPrivate,
            TotalXP = workout.TotalXP,
            UserProfileId = workout.UserProfileId
        }).ToList();

        return workoutDtos;
    }


    public async Task<List<WorkoutDayDto>> GetWorkoutsPage(
        Guid userId,
        string? authorName,
        bool onlyMy,
        bool byDescending,
        int page)
    {
        int pageSize = 10; 

        var workouts = await _unitOfWork.WorkoutRepository
            .GetWorkoutsPageAsync(userId, authorName, onlyMy, byDescending, page, pageSize);

        if (workouts == null || !workouts.Any())
        {
            return new List<WorkoutDayDto>();
        }

        var groupedWorkouts = workouts
            .GroupBy(w => w.CreatedAt.Date)
            .Select(group => new WorkoutDayDto
            {
                Date = group.Key,
                WorkoutCount = group.Count(),
                TotalXpForDay = onlyMy ? group.Sum(w => w.TotalXP) : 0,
                Workouts = group
                    .OrderByDescending(w => w.CreatedAt)
                    .Select(w => new WorkoutDto
                    {
                        Id = w.Id,
                        Name = w.Name,
                        CreatedAt = w.CreatedAt,
                        UserProfileId = w.UserProfileId,
                        AuthorName = w.UserProfile?.ApplicationUser?.UserName,
                        TotalXP = onlyMy ? w.TotalXP : 0 
                    }).ToList()
            })
            .ToList();

        if (byDescending)
        {
            groupedWorkouts = groupedWorkouts.OrderByDescending(d => d.Date).ToList();
        }
        else
        {
            groupedWorkouts = groupedWorkouts.OrderBy(d => d.Date).ToList();
        }

        return groupedWorkouts;
    }

    public async Task<WorkoutDetailsViewModel> GetWorkoutDetailsViewModel(Guid currentUserId, Guid workoutId)
    {
        var workout = await _unitOfWork.WorkoutRepository.GetByIdAsync(workoutId);
        if (workout == null) return null; // це треба якось хендлити на фронті

        if (workout.IsPrivate && workout.UserProfileId != currentUserId)
        {
            return null; // це треба якось хендлити на фронті
        }

        var workoutAuthor = await _unitOfWork.UserProfileRepository.GetAllCredentialsAboutUserByIdAsync(workout.UserProfileId);

        if (workoutAuthor == null) throw new NullReferenceException("workoutAuthor was null");

        var exerciseEntities = await _unitOfWork.UserExerciseRepository
            .GetAllByWorkoutIdAsync(workoutId);


        var exerciseDtos = exerciseEntities.Select(e => new UserExerciseDto
        {
            Id = e.Id,
            WorkoutId = e.WorkoutId,
            Name = e.Name,
            Sets = e.Sets,
            Reps = e.Reps,
            Weight = e.Weight,
            Duration = e.Duration,
            EarnedXP = e.EarnedXP
        }).ToList();

        return new WorkoutDetailsViewModel
        {
            WorkoutId = workout.Id,
            Name = workout.Name,
            Description = workout.Description,
            Conclusion = workout.Conclusion,
            AuthorName = workoutAuthor?.ApplicationUser?.UserName ?? "Unknown",
            AuthorId = workout.UserProfileId,
            CreatedAt = workout.CreatedAt,
            TotalXP = workout.TotalXP,
            IsPrivate = workout.IsPrivate,

            IsOwner = (workout.UserProfileId == currentUserId),

            Exercises = exerciseDtos,

            Comments = new CommentsSectionViewModel
            {
                TargetId = workout.Id,
                TargetType = Data.Enums.CommentTargetType.Workout,
                //Items = await _unitOfWork.CommentRepository.(workout.Id) 
            }
        };
    }
    public async Task UpdateWorkoutInfoAsync(UpdateWorkoutRequestDto dto, Guid userId)
    {
        var workout = await _unitOfWork.WorkoutRepository.GetByIdAsync(dto.Id);

        if (workout == null) throw new Exception("Workout not found");
        if (workout.UserProfileId != userId) throw new Exception("Access denied");

        workout.Name = dto.Name;
        workout.Description = dto.Description;
        workout.Conclusion = dto.Conclusion;
        workout.IsPrivate = dto.IsPrivate;

        await _unitOfWork.WorkoutRepository.UpdateAsync(workout);
        await _unitOfWork.SaveAsync();
    }
    
    public async Task RemoveWorkoutAsync(Guid userId, Guid workoutId)
    {
        var workout = await _unitOfWork.WorkoutRepository.GetByIdAsync(workoutId);
        if (workout.UserProfileId != userId) throw new Exception("Access denided");

        await _unitOfWork.WorkoutRepository.DeleteByIdAsync(workoutId);
        await _unitOfWork.SaveAsync();
    }
}
