using Gymify.Application.DTOs.Comment;
using Gymify.Application.DTOs.Exercise;
using Gymify.Application.DTOs.User;

namespace Gymify.Application.Services.Interfaces;

public interface IAdminService
{
    Task<List<ExerciseDto>> GetUnapprovedExercisesAsync();
    Task ApproveExerciseAsync(UpdateExerciseRequestDto updatedExercise);
    Task RejectExerciseAsync(Guid id, string reason);
    Task<List<CommentAdminDto>> GetUnapprovedCommentsAsync();
    Task ApproveCommentAsync(Guid id);
    Task RejectCommentAsync(Guid id, string reason);
    Task UpdateCommentAsync(Guid id, string content);
    Task<List<UserAdminDto>> GetAllUsersAsync();
    Task ToggleBanAsync(Guid id);
    Task ChangeRoleAsync(Guid id, string role);
    Task DeleteUserAsync(Guid id);
}
