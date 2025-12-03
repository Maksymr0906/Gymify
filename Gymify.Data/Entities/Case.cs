using Gymify.Data.Entities;
using Gymify.Data.Enums;

public class Case : BaseEntity
{
    public string NameEn { get; set; } = string.Empty;
    public string NameUk { get; set; } = string.Empty;
    public string DescriptionEn { get; set; } = string.Empty;
    public string DescriptionUk { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public CaseType Type { get; set; }
    public ICollection<UserCase> UserCases { get; set; } = [];
}
