using Gymify.Data.Entities;

namespace Gymify.Data.Interfaces.Repositories;

public interface IUserEquipmentRepository : IRepository<UserEquipment>
{
    Task<UserEquipment> GetByUserIdAsync(Guid userProfileId);
}
