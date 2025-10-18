namespace Gymify.Application.Services.Interfaces;

public interface IExerciseService
{
    Task<int> GetBaseXpForExerciseAsync(string name);
}
