using Gymify.Application.DTOs.Workout;
using Gymify.Application.DTOs.WorkoutsFeed;
using Gymify.Application.ViewModels.Workout;
using Gymify.Data.Entities;
using System;

namespace Gymify.Application.Services.Interfaces;

public interface IWorkoutService
{
    Task<WorkoutDto> CreateWorkoutAsync(CreateWorkoutRequestDto model, Guid userProfileId);
    Task<CompleteWorkoutResponseDto> CompleteWorkoutAsync(CompleteWorkoutRequestDto model);
    Task<ICollection<WorkoutDto>> GetAllUserWorkoutsAsync(Guid userProfileId);
    Task<List<WorkoutDayDto>> GetWorkoutsPage(
    Guid userId,
    string? authorName,
    bool onlyMy,
    bool byDescending,
    int page);
    Task<WorkoutDetailsViewModel> GetWorkoutDetailsViewModel(Guid currentUserId, Guid workoutId);
    Task UpdateWorkoutInfoAsync(UpdateWorkoutRequestDto dto, Guid userId);
}
