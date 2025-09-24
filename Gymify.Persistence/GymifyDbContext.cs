using Gymify.Data.Entities;
using Gymify.Persistence.Configurations;
using Gymify.Persistence.SeedData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Gymify.Persistence;

public class GymifyDbContext(
    DbContextOptions<GymifyDbContext> options,
    IOptions<AuthorizationOptions> authOptions,
    IOptions<SeedDataOptions> seedDataOptions)
    : DbContext(options)
{
    private readonly AuthorizationOptions _authorizationOptions = authOptions.Value;
    private readonly SeedDataOptions _seedDataOptions = seedDataOptions.Value;

    public DbSet<Achievement> Achievements { get; set; }
    public DbSet<Case> Cases { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<FriendInvite> FriendInvites { get; set; }
    public DbSet<Friendship> Friendships { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserAchievement> UserAchievements { get; set; }
    public DbSet<UserCase> UserCases { get; set; }
    public DbSet<UserEquipment> UserEquipments { get; set; }
    public DbSet<UserExercise> UserExercises { get; set; }
    public DbSet<UserItem> UserItems { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Workout> Workouts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GymifyDbContext).Assembly);

        modelBuilder.ApplyConfiguration(new RolePermissionConfiguration(_authorizationOptions));

        modelBuilder.ApplyConfiguration(new UserConfiguration(_seedDataOptions));

        modelBuilder.ApplyConfiguration(new UserRoleConfiguration(_seedDataOptions));
    }
}
