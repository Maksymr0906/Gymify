namespace Gymify.Persistence.SeedData.Models;

public record ApplicationUserSeedData
{
    public required Guid Id { get; init; }
    public required Guid UserProfileId { get; set; }
    public required string UserName { get; init; }
    public required string NormalizedUserName { get; init; }
    public required string Email { get; init; }
    public required string NormalizedEmail { get; init; }
    public required string PasswordHash { get; init; }

    // Додаткові обов’язкові поля для Identity
    public bool EmailConfirmed { get; init; } = true;
    public string SecurityStamp { get; init; } = Guid.NewGuid().ToString("D");
    public string ConcurrencyStamp { get; init; } = Guid.NewGuid().ToString("D");
    public string? PhoneNumber { get; init; } = null;
    public bool PhoneNumberConfirmed { get; init; } = false;
    public bool TwoFactorEnabled { get; init; } = false;
    public DateTimeOffset? LockoutEnd { get; init; } = null;
    public bool LockoutEnabled { get; init; } = true;
    public int AccessFailedCount { get; init; } = 0;
}
