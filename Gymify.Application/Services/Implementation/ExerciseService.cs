using Gymify.Application.DTOs.Exercise;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Application.Services.Implementation;

public class ExerciseService(IUnitOfWork unitOfWork) : IExerciseService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IEnumerable<ExerciseDto>> FindByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Enumerable.Empty<ExerciseDto>();

        var exercises = await _unitOfWork.ExerciseRepository.FindByNameAsync(name);

        if (exercises == null || !exercises.Any())
            return Enumerable.Empty<ExerciseDto>();

        var approved = exercises.Where(e => e.IsApproved);

        if (!approved.Any())
            return Enumerable.Empty<ExerciseDto>();

        return approved.Select(e => new ExerciseDto
        {
            Id = e.Id,
            Name = e.Name,
            Type = (int)e.Type,
            Description = e.Description,
            VideoURL = e.VideoURL,
        });
    }


    public Task<int> GetBaseXpForExerciseAsync(string name)
    {
        throw new NotImplementedException();
    }
}
