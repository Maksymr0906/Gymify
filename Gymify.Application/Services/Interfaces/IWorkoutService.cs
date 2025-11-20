using Gymify.Application.DTOs.Workout;
using Gymify.Application.DTOs.WorkoutsFeed;
using Gymify.Data.Entities;
using System;

namespace Gymify.Application.Services.Interfaces;

public interface IWorkoutService
{
    Task<WorkoutDto> CreateWorkoutAsync(CreateWorkoutRequestDto model, Guid userProfileId);
    Task<CompleteWorkoutResponseDto> CompleteWorkoutAsync(CompleteWorkoutRequestDto model);
    Task<ICollection<WorkoutDto>> GetAllUserWorkoutsAsync(Guid userProfileId);
    Task<DateTime?> GetFirstWorkoutDate(Guid userId, bool onlyMy, string authorName);
    Task<List<WorkoutDayDto>> GetWorkoutsByDayPage(DateTime? anchorDate, Guid userId, string? authorName, int page, bool onlyMy, bool byDescending);
    Task<List<WorkoutDayDto>> GetWorkoutsPage(
    Guid userId,
    string? authorName,
    bool onlyMy,
    bool byDescending,
    int page);
}
