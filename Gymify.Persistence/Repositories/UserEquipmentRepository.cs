using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Persistence.Repositories;

public class UserEquipmentRepository(GymifyDbContext context)
    : Repository<UserEquipment>(context), IUserEquipmentRepository
{
}
