using Gymify.Data.Entities;
using Gymify.Persistence.Configurations;
using Gymify.Persistence.SeedData;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Gymify.Persistence;

public class GymifyDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    private readonly SeedDataOptions _seedDataOptions;

    public GymifyDbContext(
        DbContextOptions<GymifyDbContext> options,
        IOptions<SeedDataOptions> seedDataOptions)
        : base(options)
    {
        _seedDataOptions = seedDataOptions.Value;
    }

    public DbSet<Achievement> Achievements { get; set; }
    public DbSet<Case> Cases { get; set; }
    public DbSet<CaseItem> CaseItems { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Exercise> Exercises { get; set; }

    public DbSet<FriendInvite> FriendInvites { get; set; }
    public DbSet<Friendship> Friendships { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<UserChat> UserChats { get; set; }
    public DbSet<MessageReadStatus> MessageReadStatuses { get; set; }

    public DbSet<Image> Images { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<UserAchievement> UserAchievements { get; set; }
    public DbSet<UserCase> UserCases { get; set; }
    public DbSet<UserEquipment> UserEquipments { get; set; }
    public DbSet<UserExercise> UserExercises { get; set; }
    public DbSet<UserItem> UserItems { get; set; }
    public DbSet<Workout> Workouts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GymifyDbContext).Assembly);

        modelBuilder.ApplyConfiguration(new AchievementConfiguration(_seedDataOptions));

        modelBuilder.ApplyConfiguration(new CaseConfiguration(_seedDataOptions));

        modelBuilder.ApplyConfiguration(new CaseItemConfiguration(_seedDataOptions));

        modelBuilder.ApplyConfiguration(new ExerciseConfiguration(_seedDataOptions));

        modelBuilder.ApplyConfiguration(new ImageConfiguration(_seedDataOptions));

        modelBuilder.ApplyConfiguration(new ItemConfiguration(_seedDataOptions));

        modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration(_seedDataOptions));

        modelBuilder.ApplyConfiguration(new UserProfileConfiguration(_seedDataOptions));

        modelBuilder.ApplyConfiguration(new WorkoutConfiguration(_seedDataOptions));

        modelBuilder.ApplyConfiguration(new NotificationConfiguration(_seedDataOptions));

        modelBuilder.ApplyConfiguration(new UserEquipmentConfiguration(_seedDataOptions));

        modelBuilder.ApplyConfiguration(new UserAchievementConfiguration(_seedDataOptions));

        modelBuilder.ApplyConfiguration(new UserExerciseConfiguration(_seedDataOptions));

        modelBuilder.ApplyConfiguration(new UserItemConfiguration(_seedDataOptions));

        modelBuilder.ApplyConfiguration(new UserCaseConfiguration(_seedDataOptions));
    }
}
