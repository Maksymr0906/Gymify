using Gymify.Application.DTOs.Exercise;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Application.Services.Implementation;

public class AdminService : IAdminService
{
    private readonly IUnitOfWork _unitOfWork;

    public AdminService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task ApproveExerciseAsync(UpdateExerciseRequestDto updatedExercise)
    {
        var exercise = await _unitOfWork.ExerciseRepository.GetByIdAsync(updatedExercise.Id);

        if (exercise == null)
            throw new Exception("Exercise not found.");

        exercise.Name = updatedExercise.Name;
        exercise.Description = updatedExercise.Description;
        exercise.BaseXP = updatedExercise.BaseXP;
        exercise.DifficultyMultiplier = updatedExercise.DifficultyMultiplier;
        exercise.Type = updatedExercise.Type;
        exercise.VideoURL = updatedExercise.VideoURL;
        exercise.IsApproved = true;
        exercise.IsRejected = false;

        await _unitOfWork.ExerciseRepository.UpdateAsync(exercise);
        await _unitOfWork.SaveAsync();
    }

    public async Task<List<ExerciseDto>> GetUnapprovedExercisesAsync()
    {
        var exercises = await _unitOfWork.ExerciseRepository.GetUnapprovedAsync();

        var dtos = exercises.Select(e => new ExerciseDto
        {
            Id = e.Id,
            Name = e.Name,
            Description = e.Description,
            BaseXP = e.BaseXP,
            DifficultyMultiplier = e.DifficultyMultiplier,
            Type = e.Type,
            IsApproved = e.IsApproved,
            IsRejected = e.IsRejected,
            VideoURL = e.VideoURL
        }).ToList();

        return dtos;
    }

    public async Task RejectExerciseAsync(Guid id, string reason)
    {
        var exercise = await _unitOfWork.ExerciseRepository.GetByIdAsync(id);

        if (exercise == null)
            throw new Exception("Exercise not found.");

        exercise.IsApproved = false;
        exercise.IsRejected = true;

        await _unitOfWork.ExerciseRepository.UpdateAsync(exercise);
        await _unitOfWork.SaveAsync();
    }
}