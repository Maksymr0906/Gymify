using Gymify.Application.Services.Implementation;
using Gymify.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Gymify.Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(AutomapperProfile));

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAchievementService, AchievementService>();
        services.AddScoped<ICaseService, CaseService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IExerciseService, ExerciseService>();
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<IItemService, ItemService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IUserEquipmentService, UserEquipmentService>();
        services.AddScoped<IUserExersiceService, UserExerciseService>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<IWorkoutService, WorkoutService>();

        return services;
    }
}
