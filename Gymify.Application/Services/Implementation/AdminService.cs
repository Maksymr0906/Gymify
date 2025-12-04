using Gymify.Application.DTOs.Comment;
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

    public async Task ApproveExerciseAsync(UpdateExerciseRequestDto updatedExercise, bool ukranianVer)
    {
        var exercise = await _unitOfWork.ExerciseRepository.GetByIdAsync(updatedExercise.Id);

        if (exercise == null)
            throw new Exception("Exercise not found.");


        if (ukranianVer)
        {
            exercise.NameUk = updatedExercise.Name;
            exercise.DescriptionUk = updatedExercise.Description;
        }
        else
        {
            exercise.NameEn = updatedExercise.Name;
            exercise.DescriptionEn = updatedExercise.Description;
        }
        
        exercise.BaseXP = updatedExercise.BaseXP;
        exercise.DifficultyMultiplier = updatedExercise.DifficultyMultiplier;
        exercise.Type = updatedExercise.Type;
        exercise.VideoURL = updatedExercise.VideoURL;
        exercise.IsApproved = true;
        exercise.IsRejected = false;

        await _unitOfWork.ExerciseRepository.UpdateAsync(exercise);
        await _unitOfWork.SaveAsync();
    }

    public async Task<List<ExerciseDto>> GetUnapprovedExercisesAsync(bool ukranianVer)
    {
        var exercises = await _unitOfWork.ExerciseRepository.GetUnapprovedAsync();

        var dtos = exercises.Select(e => new ExerciseDto
        {
            Id = e.Id,
            Name = ukranianVer ? e.NameUk : e.NameEn,
            Description = ukranianVer ? e.DescriptionUk : e.DescriptionEn,
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

    public async Task ApproveCommentAsync(Guid id)
    {
        var comment = await _unitOfWork.CommentRepository.GetByIdAsync(id)
                ?? throw new Exception("Comment not found");

        comment.IsApproved = true;
        comment.IsRejected = false;

        await _unitOfWork.CommentRepository.UpdateAsync(comment);
        await _unitOfWork.SaveAsync();
    }

    public async Task<List<CommentAdminDto>> GetUnapprovedCommentsAsync()
    {
        var comments = await _unitOfWork.CommentRepository.GetUnapprovedAsync();

        return comments.Select(c => new CommentAdminDto
        {
            Id = c.Id,
            Content = c.Content,
            AuthorName = c.Author?.ApplicationUser?.UserName ?? "Unknown",
            TargetId = c.TargetId,
            TargetType = c.TargetType
        }).ToList();
    }

    public async Task RejectCommentAsync(Guid id, string reason)
    {
        var comment = await _unitOfWork.CommentRepository.GetByIdAsync(id)
                ?? throw new Exception("Comment not found");

        comment.IsRejected = true;
        comment.IsApproved = false;
        comment.RejectionReason = reason;

        await _unitOfWork.CommentRepository.UpdateAsync(comment);
        await _unitOfWork.SaveAsync();
    }

    public async Task UpdateCommentAsync(Guid id, string content)
    {
        var comment = await _unitOfWork.CommentRepository.GetByIdAsync(id)
                ?? throw new Exception("Comment not found");

        comment.Content = content;

        await _unitOfWork.CommentRepository.UpdateAsync(comment);
        await _unitOfWork.SaveAsync();
    }
}