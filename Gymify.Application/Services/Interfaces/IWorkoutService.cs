using Gymify.Application.DTOs.Workout;
using Gymify.Data.Entities;

namespace Gymify.Application.Services.Interfaces;

public interface IWorkoutService
{
    Task<WorkoutDto> CreateWorkoutAsync(CreateWorkoutRequestDto model, Guid userProfileId);
    Task<CompleteWorkoutResponseDto> CompleteWorkoutAsync(CompleteWorkoutRequestDto model);
}
