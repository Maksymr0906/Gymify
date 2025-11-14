using Gymify.Application.DTOs.Workout;
using Gymify.Application.Services.Implementation;
using Gymify.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Web.Controllers
{
    public class WorkoutsCalendarController : Controller
    {
        public IWorkoutService _workoutService;

        public WorkoutsCalendarController(IWorkoutService workoutService)
        {
            _workoutService = workoutService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? authorName, bool onlyMy = true, int page = 0)
        {
            var userId = Guid.Parse(User.FindFirst("UserProfileId")!.Value);

            var model = await _workoutService.GetWorkoutsByDayPage(userId, authorName, page, onlyMy);

            ViewBag.OnlyMy = onlyMy;
            ViewBag.AuthorName = authorName;

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> LoadMoreWorkouts(string? authorName, bool onlyMy, int page)
        {
            var userId = Guid.Parse(User.FindFirst("UserProfileId")!.Value);

            var model = await _workoutService.GetWorkoutsByDayPage(userId, authorName, page, onlyMy);

            ViewBag.OnlyMy = onlyMy;
            ViewBag.AuthorName = authorName;

            return PartialView("WorkoutsList", model);
        }
    }

}
