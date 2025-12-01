using Gymify.Application.DTOs.Exercise;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Enums;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Application.Services.Implementation;

public class ExerciseService(IUnitOfWork unitOfWork) : IExerciseService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IEnumerable<ExerciseDto>> FindByNameAsync(string name, bool ukranianVer)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Enumerable.Empty<ExerciseDto>();

        var exercises = await _unitOfWork.ExerciseRepository.FindByNameAsync(name, ukranianVer);

        if (exercises == null || !exercises.Any())
            return Enumerable.Empty<ExerciseDto>();

        var approved = exercises.Where(e => e.IsApproved);

        if (!approved.Any())
            return Enumerable.Empty<ExerciseDto>();

        return approved.Select(e => new ExerciseDto
        {
            Id = e.Id,
            Name = ukranianVer ? e.NameUk : e.NameEn,
            Description = ukranianVer ? e.DescriptionUk : e.DescriptionEn,
            Type = e.Type,
            VideoURL = e.VideoURL,
        });
    }


    public Task<int> GetBaseXpForExerciseAsync(string name)
    {
        throw new NotImplementedException();
    }

    public async Task<(List<ExerciseDto> Exercises, int TotalPages)> GetFilteredExercisesAsync(
        string? search,
        ExerciseType? type,
        bool pendingOnly,
        int page,
        int pageSize,
        bool ukranianVer)
    {
        // 1. Отримуємо дані з БД
        var (entities, totalCount) = await _unitOfWork.ExerciseRepository
            .GetFilteredAsync(search, type, pendingOnly, page, pageSize, ukranianVer);

        // 2. Розрахунок сторінок
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        // 3. Маппінг в DTO
        var dtos = entities.Select(e => new ExerciseDto
        {
            Id = e.Id,
            Name = ukranianVer ? e.NameUk : e.NameEn,
            Description = ukranianVer ? e.DescriptionUk : e.DescriptionEn,
            Type = e.Type,
            VideoURL = e.VideoURL, // Важливо для YouTube хелпера
            IsApproved = e.IsApproved,
            BaseXP = e.BaseXP
        }).ToList();

        return (dtos, totalPages);
    }
}
