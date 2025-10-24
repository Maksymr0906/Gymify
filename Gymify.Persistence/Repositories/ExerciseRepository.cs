using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Persistence.Repositories;

public class ExerciseRepository(GymifyDbContext context)
    : Repository<Exercise>(context), IExerciseRepository
{
    public async Task<IEnumerable<Exercise>> FindByNameAsync(string name)
    {
        return await Entities
            .Where(e => e.Name.Contains(name))
            .ToListAsync();
    }

    public async Task<Exercise> GetByNameAsync(string name)
    {
        return await Entities.FirstOrDefaultAsync(e => e.Name.ToLower() == name.ToLower());
    }
}
