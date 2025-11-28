using Gymify.Application.DTOs.Exercise;

namespace Gymify.Application.Services.Interfaces;

public interface IAdminService
{
    Task<List<ExerciseDto>> GetUnapprovedExercisesAsync();
    Task ApproveExerciseAsync(UpdateExerciseRequestDto updatedExercise);
    Task RejectExerciseAsync(Guid id, string reason);
}
