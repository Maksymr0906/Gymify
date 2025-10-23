using Gymify.Application.DTOs.Exercise;

namespace Gymify.Application.Services.Interfaces;

public interface IExerciseService
{
    Task<int> GetBaseXpForExerciseAsync(string name);
    Task<IEnumerable<ExerciseDto>> FindByNameAsync(string name);
}
