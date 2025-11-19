using Gymify.Application.DTOs.UserExercise;

namespace Gymify.Application.Services.Interfaces;

public interface IUserExersiceService
{
    Task<UserExerciseDto> AddUserExerciseToWorkoutAsync(AddUserExerciseToWorkoutRequestDto model, Guid currentUserId);
    Task AddExercisesBatchAsync(Guid workoutId, List<AddUserExerciseToWorkoutRequestDto> exercises, Guid currentUserId);
}
