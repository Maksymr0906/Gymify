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
            .FirstOrDefaultAsync(u => u.Id == userProfileId);
    }

    public async Task UpdateUserNameAsync(Guid userProfileId, string userName)
    {
        var user = await Entities
            .Include(u => u.ApplicationUser)
            .FirstOrDefaultAsync(u => u.Id == userProfileId);

        if (user == null) throw new Exception("When we searched for application user by profileId we get null, in UpdateUserNameAsync()");

        user.ApplicationUser!.UserName = userName;

        await UpdateAsync(user); // це не так робиться, треба applicationUserRepo
    }
}
