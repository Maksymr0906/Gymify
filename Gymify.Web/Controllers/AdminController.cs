using Gymify.Application.DTOs.Exercise;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("/admin")]
    [HttpGet("/admin/home")]
    public IActionResult Home()
        => View("Home");



    // =====================
    //     EXERCISE PANEL
    // =====================
    [HttpGet("/admin/exercises")]
    public async Task<IActionResult> ExercisePanel()
    {
        var notApproved = await _adminService.GetUnapprovedExercisesAsync();
        return View(notApproved);
    }

    [HttpPost("/admin/exercises/approve")]
    public async Task<IActionResult> ApproveExercise(UpdateExerciseRequestDto updated)
    {
        await _adminService.ApproveExerciseAsync(updated);
        return RedirectToAction("ExercisePanel");
    }

    [HttpPost("/admin/exercises/reject")]
    public async Task<IActionResult> RejectExercise(Guid id, string reason)
    {
        await _adminService.RejectExerciseAsync(id, reason);
        return RedirectToAction("ExercisePanel");
    }

    // =====================
    //     COMMENT PANEL
    // =====================
    [HttpGet("/admin/comments")]
    public async Task<IActionResult> CommentPanel()
    {
        var pending = await _adminService.GetUnapprovedCommentsAsync();
        return View("CommentPanel", pending);
    }

    [HttpPost("/admin/comments/approve")]
    public async Task<IActionResult> Approve(Guid id)
    {
        await _adminService.ApproveCommentAsync(id);
        return RedirectToAction("CommentPanel");
    }

    [HttpPost("/admin/comments/reject")]
    public async Task<IActionResult> Reject(Guid id, string reason)
    {
        await _adminService.RejectCommentAsync(id, reason);
        return RedirectToAction("CommentPanel");
    }

    [HttpPost("/admin/comments/update")]
    public async Task<IActionResult> Update(Guid id, string content)
    {
        await _adminService.UpdateCommentAsync(id, content);
        return RedirectToAction("CommentPanel");
    }


    // =====================
    //       USER PANEL
    // =====================
    [HttpGet("/admin/users")]
    public async Task<IActionResult> UserPanel()
    {
        var users = await _adminService.GetAllUsersAsync();
        return View("UserPanel", users);
    }

    [HttpPost("/admin/users/ban")]
    public async Task<IActionResult> Ban(Guid id)
    {
        await _adminService.ToggleBanAsync(id);
        return RedirectToAction(nameof(UserPanel));
    }

    [HttpPost("/admin/users/role")]
    public async Task<IActionResult> ChangeRole(Guid id, string role)
    {
        await _adminService.ChangeRoleAsync(id, role);
        return RedirectToAction(nameof(UserPanel));
    }

    [HttpPost("/admin/users/delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _adminService.DeleteUserAsync(id);
        return RedirectToAction(nameof(UserPanel));
    }

    // =====================
    //     WORKOUT PANEL
    // =====================
    [HttpGet("/admin/workouts")]
    public IActionResult WorkoutPanel()
        => View("WorkoutPanel");
}
