using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Persistence.Repositories;

public class NotificationRepository(GymifyDbContext context)
    : Repository<Notification>(context), INotificationRepository
{
}
