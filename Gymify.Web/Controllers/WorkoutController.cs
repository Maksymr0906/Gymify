using Gymify.Application.DTOs.Workout;
using Gymify.Application.DTOs.UserExercise;
using Gymify.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Gymify.Web.Controllers
{
    public class WorkoutController : Controller
    {
        private readonly IWorkoutService _workoutService;
        private readonly IExerciseService _exerciseService;
        private readonly IUserExersiceService _userExerciseService;

        public WorkoutController(
            IWorkoutService workoutService,
            IExerciseService exerciseService,
            IUserExersiceService userExerciseService)
        {
            _workoutService = workoutService;
            _exerciseService = exerciseService;
            _userExerciseService = userExerciseService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("Create");
        }

        [HttpPost]
        public async Task<IActionResult> GenerateWorkout(CreateWorkoutRequestDto dto)
        {
            var userProfileId = Guid.Parse(User.FindFirst("UserProfileId")?.Value ?? Guid.Empty.ToString());

            var workout = await _workoutService.CreateWorkoutAsync(dto, userProfileId);

            TempData["WorkoutId"] = workout.Id.ToString();
            return RedirectToAction("AddExercise", "Workout");
        }

        [HttpGet]
        public IActionResult AddExercise()
        {
            var workoutId = TempData["WorkoutId"]?.ToString();
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

            await _userExerciseService.AddUserExerciseToWorkoutAsync(dto, currentUserId);

            if (action == "end")
                return RedirectToAction("Finish", "Workout");
            else if (action == "add")
                return RedirectToAction("AddExercise", "Workout");

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

        [HttpGet]
        public async Task<IActionResult> Finish(CompleteWorkoutRequestDto dto)
        {
            await _workoutService.CompleteWorkoutAsync(dto);
            return View("Finish");
        }
    }
}
