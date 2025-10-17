using Gymify.Application.DTOs.Workout;
using Gymify.Application.Services.Interfaces;

namespace Gymify.Application.Services.Implementation;

public class WorkoutService : IWorkoutService
{
    public Task<WorkoutDto> CompleteWorkoutAsync(CompleteWorkoutRequestDto model)
    {
        throw new NotImplementedException();
    }

    public Task<WorkoutDto> CreateWorkoutAsync(CreateWorkoutRequestDto model)
    {
        throw new NotImplementedException();
    }
}
