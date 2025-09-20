namespace Gymify.Data.Entities;

public class Exercise : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string VideoURL { get; set; } = string.Empty;
    public int BaseXP { get; set; } = 0;
    public double DifficultyMultiplier { get; set; } = 0;
}
