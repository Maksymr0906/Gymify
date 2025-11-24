using Gymify.Application.DTOs.UserExercise;
using Gymify.Application.DTOs.Workout;
using Gymify.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Gymify.Web.Controllers
{
    [Authorize]
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
            return RedirectToAction("AddExercise");
        }

        [HttpGet]
        public async Task<IActionResult> AddExercise(Guid? workoutId)
        {
            var id = workoutId ?? (TempData["WorkoutId"] != null ? Guid.Parse(TempData["WorkoutId"].ToString()) : Guid.Empty);

            if (id == Guid.Empty) return RedirectToAction("Create");

            ViewBag.WorkoutId = id;
            TempData.Keep("WorkoutId");

            return View(new List<UserExerciseDto>());
        }

        [HttpPost]
        public async Task<IActionResult> AddExercisesBatch(Guid workoutId, List<UserExerciseDto> exercises)
        {
            var currentUserId = Guid.Parse(User.FindFirst("UserProfileId")?.Value ?? Guid.Empty.ToString());

            await _userExerciseService.SyncWorkoutExercisesAsync(workoutId, exercises, currentUserId);

            return RedirectToAction("Finish", new { workoutId });
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
        public IActionResult Finish(Guid workoutId)
        {
            var model = new CompleteWorkoutRequestDto
            {
                WorkoutId = workoutId,
                UserProfileId = Guid.Parse(User.FindFirst("UserProfileId")?.Value ?? Guid.Empty.ToString()),
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Finish(CompleteWorkoutRequestDto dto)
        {
            await _workoutService.CompleteWorkoutAsync(dto);
            return RedirectToAction("Index", "Main");
        }
    }
}