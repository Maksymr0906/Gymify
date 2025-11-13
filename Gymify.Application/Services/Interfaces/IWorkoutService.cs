using Gymify.Application.DTOs.Workout;

namespace Gymify.Application.Services.Interfaces;

public interface IWorkoutService
{
    Task<WorkoutDto> CreateWorkoutAsync(CreateWorkoutRequestDto model);
    Task<CompleteWorkoutResponseDto> CompleteWorkoutAsync(CompleteWorkoutRequestDto model);
    Task<ICollection<WorkoutDto>> GetAllUserWorkoutsAsync(Guid userProfileId);
}
