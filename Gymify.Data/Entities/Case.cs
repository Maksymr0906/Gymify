using Gymify.Data.Entities;
using Gymify.Data.Enums;

public class Case : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public CaseType Type { get; set; }

    public ICollection<UserCase> UserCases { get; set; } = [];
}
