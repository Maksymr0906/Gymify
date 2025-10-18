using Gymify.Application.DTOs.UserExercise;
using Microsoft.AspNetCore.Mvc;

namespace Gymify.Web.Controllers
{
    public class AddTrainsController : Controller
    {
        public IActionResult Index()
        {
            return View("AddTrains");
        }
        [HttpPost]
        public IActionResult AddExercise(AddUserExerciseToWorkoutRequestDto dto, string action)
        {
            if (action == "end")
            {
                // 🔹 логіка для завершення
                return RedirectToAction("FinishWorkout");
            }
            else if (action == "add")
            {
                // 🔹 логіка для додавання ще однієї вправи
                return RedirectToAction("AddExercise");
            }

            return View();
        }
    }
}
