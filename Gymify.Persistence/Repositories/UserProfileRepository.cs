using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Persistence.Repositories;

public class UserProfileRepository(GymifyDbContext context)
    : Repository<UserProfile>(context), IUserProfileRepository
{
    private readonly GymifyDbContext _context = context;
    public async Task<UserProfile> GetByApplicationUserId(Guid applicationUserId)
    {
        return await Entities.FirstOrDefaultAsync(x => x.ApplicationUserId == applicationUserId);
    }

    public async Task<UserProfile?> GetAllCredentialsAboutUserByIdAsync(Guid userProfileId)
    {
        return await Entities
            .Include(u => u.ApplicationUser)
            .Include(u => u.Equipment).ThenInclude(ue => ue.Avatar)
            .FirstOrDefaultAsync(u => u.Id == userProfileId);
    }
    public async Task<(List<UserProfile> Users, int TotalCount)> GetLeaderboardPageAsync(int page, int pageSize)
    {
        // 1. Спочатку дістаємо ID ролі "Admin" з бази (динамічно)
        // Вам потрібен доступ до RoleManager або просто до контексту (DbContext)
        var adminRoleId = _context.Roles
            .Where(r => r.Name == "Admin")
            .Select(r => r.Id)
            .FirstOrDefault();

        // 2. Тепер використовуємо цей ID у вашому запиті
        var query = Entities
            .AsNoTracking()
            .Include(u => u.ApplicationUser)
            .Include(u => u.Equipment).ThenInclude(e => e.Avatar)
            .OrderByDescending(u => u.CurrentXP);

        var totalCount = await query.CountAsync();

        var users = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (users, totalCount);
    }

    public async Task<int> GetUserRankByXpAsync(long userXp)
    {
        var countBetter = await Entities.CountAsync(u => u.CurrentXP > userXp);
        return countBetter + 1;
    }

    public async Task<List<UserProfile>> SearchUsersAsync(string searchTerm, Guid currentUserId)
    {
        // Отримуємо ID ролі Адміна (або хардкодимо, якщо знаємо, але краще так)
        // Примітка: Якщо у тебе немає доступу до Roles через контекст тут,
        // краще відфільтрувати це в Сервісі через UserManager (див. Крок 3).
        // Але якщо хочеш в репозиторії, робимо базовий пошук:

        return await Entities
            .AsNoTracking()
            .Include(u => u.ApplicationUser)
            .Include(u => u.Equipment).ThenInclude(e => e.Avatar)
            .Where(u => u.Id != currentUserId &&
                        u.ApplicationUser.UserName.Contains(searchTerm))
            .Take(20)
            .ToListAsync();
    }
}
