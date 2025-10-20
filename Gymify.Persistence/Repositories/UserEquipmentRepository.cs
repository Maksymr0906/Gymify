using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Persistence.Repositories;

public class UserEquipmentRepository(GymifyDbContext context)
    : Repository<UserEquipment>(context), IUserEquipmentRepository
{
    public async Task<UserEquipment> GetByUserIdAsync(Guid userProfileId)
    {
        return await Entities.FirstOrDefaultAsync(ue => ue.UserProfileId == userProfileId);
    }
}
