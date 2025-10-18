using Gymify.Application.DTOs.Workout;

namespace Gymify.Application.Services.Interfaces;

public interface IWorkoutService
{
    Task<WorkoutDto> CreateWorkoutAsync(CreateWorkoutRequestDto model);
    Task<WorkoutDto> CompleteWorkoutAsync(CompleteWorkoutRequestDto model);
}
