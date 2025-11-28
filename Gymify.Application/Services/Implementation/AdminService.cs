using Gymify.Application.DTOs.Exercise;
using Gymify.Application.Services.Interfaces;

namespace Gymify.Application.Services.Implementation;

public class AdminService : IAdminService
{
    public Task ApproveExerciseAsync(UpdateExerciseRequestDto updatedExercise)
    {
        throw new NotImplementedException();
    }

    public Task<List<ExerciseDto>> GetUnapprovedExercisesAsync()
    {
        throw new NotImplementedException();
    }

    public Task RejectExerciseAsync(Guid id, string reason)
    {
        throw new NotImplementedException();
    }
}