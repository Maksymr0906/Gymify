using Gymify.Data.Entities;
using Gymify.Data.Enums;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Persistence.Repositories;

public class CommentRepository(GymifyDbContext context)
    : Repository<Comment>(context), ICommentRepository
{
    private readonly GymifyDbContext _context = context;
    public async Task<ICollection<Comment>> GetCommentsByTargetIdAndTypeAsync(Guid targetId, CommentTargetType targetType)
    {
        return await _context.Comments
            .AsNoTracking() // Тільки для читання, пришвидшує запит
            .Where(c => c.TargetId == targetId && c.TargetType == targetType)
            .Include(c => c.Author).ThenInclude(u => u.ApplicationUser)
            .Include(c => c.Author).ThenInclude(u => u.Equipment).ThenInclude(ue => ue.Avatar) // <-- ВАЖЛИВО   // Підтягуємо екіпірування (для Аватара/Рамки)
            .OrderByDescending(c => c.CreatedAt)     // Сортуємо: нові зверху
            .ToListAsync();
    }
}
