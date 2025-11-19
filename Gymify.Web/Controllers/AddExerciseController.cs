using Gymify.Application.DTOs.UserExercise;
using Gymify.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gymify.Web.Controllers;

[Authorize]
public class AddExerciseController : Controller
{
    private readonly IExerciseService _exerciseService;
    private readonly IUserExersiceService _userExerciseService;

    public AddExerciseController(IExerciseService exerciseService, IUserExersiceService userExerciseService)
    {
        _exerciseService = exerciseService;
        _userExerciseService = userExerciseService;
    }

    public string? workoutId;

    [HttpGet]
    public IActionResult AddExercises()
    {
        workoutId = TempData["WorkoutId"]?.ToString();
        ViewBag.WorkoutId = workoutId;

        TempData.Keep("WorkoutId");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddExercise(AddUserExerciseToWorkoutRequestDto dto, string? newExerciseName, string action)
    {
        var currentUserId = Guid.Parse(User.FindFirst("UserProfileId")?.Value ?? Guid.Empty.ToString());
        if (!string.IsNullOrWhiteSpace(newExerciseName))
            dto.Name = newExerciseName;

        Console.WriteLine(dto.Name);
        Console.WriteLine(newExerciseName);
        Console.WriteLine(action);

        await _userExerciseService.AddUserExerciseToWorkoutAsync(dto, currentUserId);

        if (action == "end")
            return RedirectToAction("FinishWorkout", "Workout");
        else if (action == "add")
            return RedirectToAction("AddExercises");

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> SearchExercises(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Json(new List<string>());

        var exercises = await _exerciseService.FindByNameAsync(query);

        return Json(exercises.Select(e => e.Name));
    }
}
