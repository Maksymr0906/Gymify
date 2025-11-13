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

        // 1. ГОЛОВНИЙ МЕТОД: Завантажує першу сторінку (останні 28 днів)
        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var userId = Guid.Parse(User.FindFirst("UserProfileId")!.Value);
            // page: 0 означає "перша сторінка" (останні 28 днів)
            var model = await _workoutService.GetWorkoutsByDayPage(userId, 0);
            return View(model);
        }

        // 2. AJAX-МЕТОД: Завантажує наступні сторінки
        [HttpGet]
        public async Task<IActionResult> LoadMoreWorkouts(int page)
        {
            var userId = Guid.Parse(User.FindFirst("UserProfileId")!.Value);
            // 'page' буде 1, 2, 3...
            var model = await _workoutService.GetWorkoutsByDayPage(userId, page);

            // Повертаємо Partial View ТІЛЬКИ з новими даними
            return PartialView("_WorkoutDayList", model);
        }
    }

}
