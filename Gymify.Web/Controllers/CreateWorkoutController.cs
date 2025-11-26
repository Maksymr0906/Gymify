using Gymify.Application.DTOs.Workout;
using Gymify.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gymify.Web.Controllers
{
    [Authorize]
    public class CreateWorkoutController : Controller
    {
        private readonly IWorkoutService _workoutService;

        public CreateWorkoutController(IWorkoutService workoutService)
        {
            _workoutService = workoutService;
        }

        [HttpGet]
        public IActionResult CreateWorkout()
        {
            return View("CreateWorkout");
        }

        [HttpPost]
        public async Task<IActionResult> GenerateWorkout(CreateWorkoutRequestDto dto)
        {
            var currentUserId = Guid.Parse(User.FindFirst("UserProfileId")?.Value ?? Guid.Empty.ToString());
            var workout = await _workoutService.CreateWorkoutAsync(dto, currentUserId);

            TempData["WorkoutId"] = workout.Id.ToString();
            return RedirectToAction("AddExercises", "AddExercise");
        }
    }
}