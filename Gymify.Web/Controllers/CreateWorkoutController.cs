using Gymify.Application.DTOs.Workout;
using Gymify.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gymify.Web.Controllers
{
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
            var workout = await _workoutService.CreateWorkoutAsync(dto);

            TempData["WorkoutId"] = workout.Id.ToString();
            return RedirectToAction("AddTrains", "AddTrains");
        }
    }
}