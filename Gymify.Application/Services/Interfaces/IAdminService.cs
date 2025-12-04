using Gymify.Application.DTOs.Comment;
using Gymify.Application.DTOs.Exercise;

namespace Gymify.Application.Services.Interfaces;

public interface IAdminService
{
    Task<List<ExerciseDto>> GetUnapprovedExercisesAsync(bool ukranianVer);
    Task ApproveExerciseAsync(UpdateExerciseRequestDto updatedExercise, bool ukranianVer);
    Task RejectExerciseAsync(Guid id, string reason);
    Task<List<CommentAdminDto>> GetUnapprovedCommentsAsync();
    Task ApproveCommentAsync(Guid id);
    Task RejectCommentAsync(Guid id, string reason);
    Task UpdateCommentAsync(Guid id, string content);
}
