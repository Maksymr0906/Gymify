using Gymify.Application.DTOs.Exercise;
using Gymify.Data.Enums;

namespace Gymify.Application.Services.Interfaces;

public interface IExerciseService
{
    Task<int> GetBaseXpForExerciseAsync(string name);
    Task<IEnumerable<ExerciseDto>> FindByNameAsync(string name);
    Task<(List<ExerciseDto> Exercises, int TotalPages)> GetFilteredExercisesAsync(
        string? search,
        ExerciseType? type,
        bool pendingOnly,
        int page,
        int pageSize);
}
