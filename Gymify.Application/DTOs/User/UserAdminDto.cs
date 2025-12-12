namespace Gymify.Application.DTOs.User;

public class UserAdminDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public bool IsBanned { get; set; }
    public string Role { get; set; } = "";
    public int TotalXP { get; set; }
    public DateTime Registered { get; set; }
}