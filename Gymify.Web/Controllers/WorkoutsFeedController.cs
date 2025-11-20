using Gymify.Application.DTOs.Workout;
using Gymify.Application.Services.Implementation;
using Gymify.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Web.Controllers
{

    [Authorize]
    public class WorkoutsFeedController : Controller
    {
        public IWorkoutService _workoutService;

        public WorkoutsFeedController(IWorkoutService workoutService)
        {
            _workoutService = workoutService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? authorName, bool onlyMy = true, bool byDescending = true)
        {
            // Запускаємо першу сторінку (page=0)
            var model = await _workoutService.GetWorkoutsPage(
                Guid.Parse(User.FindFirst("UserProfileId")!.Value),
                authorName,
                onlyMy,
                byDescending,
                page: 0);

            ViewBag.OnlyMy = onlyMy;
            return View(model);
        }

        // ВИДАЛИТИ: [HttpGet] public async Task<IActionResult> GetAnchorDate(...)

        [HttpGet]
        public async Task<IActionResult> LoadMoreWorkouts(string? authorName, bool onlyMy, bool byDescending, int page)
        {
            var userId = Guid.Parse(User.FindFirst("UserProfileId")!.Value);

            // Викликаємо новий простий метод пагінації
            var model = await _workoutService.GetWorkoutsPage(userId, authorName, onlyMy, byDescending, page);

            ViewBag.OnlyMy = onlyMy;

            // Повертаємо PartialView. Якщо model.Count == 0, фронтенд знає, що це кінець.
            return PartialView("WorkoutsList", model);
        }

    }

}
