using Gymify.Application.DTOs.UserExercise;

namespace Gymify.Application.Services.Interfaces;

public interface IUserExersiceService
{
    Task SyncWorkoutExercisesAsync(Guid workoutId, List<AddUserExerciseDto> dtos, Guid userId, bool ukranianVer);
    Task<List<UserExerciseDto>> GetAllWorkoutExercisesAsync(Guid workoutId, bool ukranianVer);
}
