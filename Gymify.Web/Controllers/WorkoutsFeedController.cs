using Gymify.Application.DTOs.Workout;
using Gymify.Application.Services.Implementation;
using Gymify.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Web.Controllers
{
    public class WorkoutsFeedController : Controller
    {
        public IWorkoutService _workoutService;

        public WorkoutsFeedController(IWorkoutService workoutService)
        {
            _workoutService = workoutService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? anchorDate, string? authorName, bool onlyMy = true, bool byDescending = true, int page = 0)
        {
            var userId = Guid.Parse(User.FindFirst("UserProfileId")!.Value);

            var model = await _workoutService.GetWorkoutsByDayPage(anchorDate, userId, authorName, page, onlyMy, byDescending);

            ViewBag.OnlyMy = onlyMy;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAnchorDate(string? authorName, bool onlyMy)
        {
            var userId = Guid.Parse(User.FindFirst("UserProfileId")!.Value);
            var date = await _workoutService.GetFirstWorkoutDate(userId, onlyMy, authorName); 

            return Json(new { anchorDate = date });
        }

        [HttpGet]
        public async Task<IActionResult> LoadMoreWorkouts(DateTime? anchorDate, string? authorName, bool onlyMy, bool byDescending, int page) 
        {
            var userId = Guid.Parse(User.FindFirst("UserProfileId")!.Value);

            var model = await _workoutService.GetWorkoutsByDayPage(anchorDate, userId, authorName, page, onlyMy, byDescending); 

            ViewBag.OnlyMy = onlyMy;

            return PartialView("WorkoutsList", model);
        }
    }

}
