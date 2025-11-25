using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Persistence.Repositories;

public class UserProfileRepository(GymifyDbContext context)
    : Repository<UserProfile>(context), IUserProfileRepository
{
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
}
