using Gymify.Application.DTOs.Workout;

namespace Gymify.Application.DTOs.WorkoutsCalendar;

public class WorkoutDayDto
{
    public DateTime Date { get; set; }
    public List<WorkoutDto> Workouts { get; set; } = new();

    // --- Поля для вашої майбутньої "Теплової карти" ---

    // 1. Кількість тренувань за день
    public int WorkoutCount => Workouts.Count;

    // 2. Загальний досвід за день (ми порахуємо це в LINQ)
    public int TotalXpForDay { get; set; }
}
