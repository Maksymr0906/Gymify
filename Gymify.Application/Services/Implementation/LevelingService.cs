using Gymify.Application.Services.Interfaces;

namespace Gymify.Application.Services.Implementation;

public class LevelingService : ILevelingService
{
    private const double XP_PER_UNIT = 100.0;

    public int CalculateLevel(double currentXP)
    {
        return (int)Math.Floor(Math.Sqrt(currentXP / XP_PER_UNIT));
    }

    public double GetTotalXpForLevel(int level)
    {
        return Math.Pow(level, 2) * XP_PER_UNIT;
    }
}
