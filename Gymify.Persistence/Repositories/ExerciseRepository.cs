using Gymify.Data.Entities;
using Gymify.Data.Enums;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Persistence.Repositories;

public class ExerciseRepository(GymifyDbContext context)
    : Repository<Exercise>(context), IExerciseRepository
{
    public async Task<IEnumerable<Exercise>> FindByNameAsync(string name, bool ukranianVer)
    { 
        return await Entities.Where(e => ukranianVer ? e.NameUk.Contains(name) : e.NameEn.Contains(name)).ToListAsync();
    }

    public async Task<Exercise> GetByNameAsync(string name, bool ukranianVer)
    {
        return await Entities
            .FirstOrDefaultAsync(e => ukranianVer ? e.NameUk.ToLower() == name.ToLower() : e.NameEn.ToLower() == name.ToLower());
    }

    public async Task<(List<Exercise> Exercises, int TotalCount)> GetFilteredAsync(
        string? search,
        ExerciseType? type,
        bool pendingOnly,
        int page,
        int pageSize,
        bool ukranianVer)
    {
        var query = Entities.AsNoTracking();

        // 1. Пошук за назвою
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = ukranianVer ? query.Where(e => e.NameUk.Contains(search)) : query.Where(e => e.NameEn.Contains(search));
        }

        // 2. Фільтр по типу (Cardio, Strength...)
        if (type.HasValue)
        {
            query = query.Where(e => e.Type == type.Value);
        }

        // 3. Статус (Затверджено / На модерації)
        if (pendingOnly)
        {
            // Показуємо тільки ті, що НЕ затверджені
            query = query.Where(e => !e.IsApproved);
        }
        else
        {
            // Стандартний режим: показуємо тільки офіційні (затверджені)
            query = query.Where(e => e.IsApproved);
        }

        // 4. Рахуємо загальну кількість (для пагінації)
        var totalCount = await query.CountAsync();

        // 5. Отримуємо сторінку даних
        var items = await query
            .OrderBy(e => ukranianVer ? e.NameUk : e.NameEn) // Сортуємо за алфавітом
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
