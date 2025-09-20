﻿namespace Gymify.Data.Entities;

public class Friendship
{
    public Guid UserId1 { get; set; }
    public Guid UserId2 { get; set; }
    public User User1 { get; set; } = null!;
    public User User2 { get; set; } = null!;
}
