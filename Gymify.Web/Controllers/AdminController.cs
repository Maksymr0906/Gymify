using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    // =====================
    //       HOME
    // =====================
    [HttpGet("/admin")]
    [HttpGet("/admin/home")]
    public IActionResult Home()
        => View("Home");



    // =====================
    //     COMMENT PANEL
    // =====================
    [HttpGet("/admin/comments")]
    public IActionResult CommentPanel()
        => View("CommentPanel");



    // =====================
    //     EXERCISE PANEL
    // =====================
    [HttpGet("/admin/exercises")]
    public IActionResult ExercisePanel()
        => View("ExercisePanel");



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
