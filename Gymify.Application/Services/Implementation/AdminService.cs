using Gymify.Application.DTOs.Comment;
using Gymify.Application.DTOs.Exercise;
using Gymify.Application.DTOs.User;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Application.Services.Implementation;

public class AdminService : IAdminService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;

    public AdminService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
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

    public async Task<List<UserAdminDto>> GetAllUsersAsync()
    {
        var users = await _userManager.Users
            .Include(u => u.UserProfile)
            .ToListAsync();

        var result = new List<UserAdminDto>();

        foreach (var u in users)
        {
            var roles = await _userManager.GetRolesAsync(u);
            result.Add(new UserAdminDto
            {
                Id = u.Id,
                Username = u.UserName ?? "",
                Email = u.Email ?? "",
                Role = roles.FirstOrDefault() ?? "User",
                IsBanned = u.LockoutEnd.HasValue && u.LockoutEnd.Value > DateTimeOffset.Now,
                TotalXP = (int)(u.UserProfile?.CurrentXP ?? 0),
                Registered = u.UserProfile?.CreatedAt ?? DateTime.Now
            });
        }

        return result;
    }

    public async Task ToggleBanAsync(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return;

        if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.Now)
        {
            // Unban
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
        }
        else
        {
            // Ban indefinitely
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
        }
    }

    public async Task ChangeRoleAsync(Guid id, string role)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return;

        var currentRoles = await _userManager.GetRolesAsync(user);

        await _userManager.RemoveFromRolesAsync(user, currentRoles);

        await _userManager.AddToRoleAsync(user, role);
    }

    public async Task DeleteUserAsync(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return;

        var profile = await _unitOfWork.UserProfileRepository.GetByIdAsync(user.UserProfileId);
        if (profile != null)
        {
            await _unitOfWork.UserProfileRepository.DeleteByIdAsync(profile.Id);
        }

        await _userManager.DeleteAsync(user);
        await _unitOfWork.SaveAsync();
    }
}