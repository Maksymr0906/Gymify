using Gymify.Data.Interfaces.Repositories;
using Gymify.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gymify.Persistence;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<GymifyDbContext>(options =>
        {
            options.UseMySql(configuration.GetConnectionString("GymifyConnectionString"),
                new MySqlServerVersion(new Version(8, 3, 0)));
        });

        services.AddScoped<IAchievementRepository, AchievementRepository>();
        services.AddScoped<ICaseRepository, CaseRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IExerciseRepository, ExerciseRepository>();
        services.AddScoped<IImageRepository, ImageRepository>();
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IUserEquipmentRepository, UserEquipmentRepository>();
        services.AddScoped<IUserExerciseRepository, UserExerciseRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IWorkoutRepository, WorkoutRepository>();
        services.AddScoped<IUserCaseRepository, UserCaseRepository>();
        services.AddScoped<IPendingExerciseRepository, PendingExerciseRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
