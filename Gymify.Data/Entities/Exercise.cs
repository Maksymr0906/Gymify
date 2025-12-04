using Gymify.Data.Enums;

namespace Gymify.Data.Entities;

public class Exercise : BaseEntity
{
    public string NameEn { get; set; } = string.Empty;
    public string NameUk { get; set; } = string.Empty;
    public string DescriptionEn { get; set; } = string.Empty;
    public string DescriptionUk { get; set; } = string.Empty;
    public ExerciseType Type { get; set; }
    public string VideoURL { get; set; } = string.Empty;
    public int BaseXP { get; set; } = 0;
    public double DifficultyMultiplier { get; set; } = 0;
    public bool IsApproved { get; set; } = false;
    public bool IsRejected { get; set; } = false;
    public string? RejectReason { get; set; }
}
