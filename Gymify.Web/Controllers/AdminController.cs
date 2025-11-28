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

    // =====================
    //       HOME
    // =====================
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
    public IActionResult CommentPanel()
        => View("СommentPanel");



    // =====================
    //       USER PANEL
    // =====================
    [HttpGet("/admin/users")]
    public IActionResult UserPanel()
        => View("UserPanel");



    // =====================
    //     WORKOUT PANEL
    // =====================
    [HttpGet("/admin/workouts")]
    public IActionResult WorkoutPanel()
        => View("WorkoutPanel");
}
