using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Persistence.Repositories;

public class ImageRepository(GymifyDbContext context)
    : Repository<Image>(context), IImageRepository
{
}
