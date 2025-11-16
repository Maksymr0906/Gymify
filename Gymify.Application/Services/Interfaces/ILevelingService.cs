namespace Gymify.Application.Services.Interfaces;

public interface ILevelingService
{
    int CalculateLevel(double currentXP);
    double GetTotalXpForLevel(int level);
}