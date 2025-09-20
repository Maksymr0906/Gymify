namespace Gymify.Data.Entities;

public class Case : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public double DropChance { get; set; } = 0;
    public ICollection<UserCase> UserCases { get; set; } = [];
}
