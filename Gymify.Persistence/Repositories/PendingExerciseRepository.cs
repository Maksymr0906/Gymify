using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gymify.Persistence.Repositories;

public class PendingExerciseRepository(GymifyDbContext context) 
    : Repository<PendingExercise>(context), IPendingExerciseRepository
{
    public async Task<PendingExercise> GetByNameAsync(string name)
    {
        return await Entities.FirstOrDefaultAsync(pe => pe.Name == name);
    }
}
