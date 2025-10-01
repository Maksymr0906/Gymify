namespace Gymify.Data.Interfaces.Repositories;

public interface IUnitOfWork
{
    IAchievementRepository AchievementRepository { get; }
    ICaseRepository CaseRepository { get; }
    ICommentRepository CommentRepository { get; }
    IExerciseRepository ExerciseRepository { get; }
    IFriendInviteRepository FriendInviteRepository { get; }
    IFriendshipRepository FriendshipRepository { get; }
    IItemRepository ItemRepository { get; }
    IMessageRepository MessageRepository { get; }
    INotificationRepository NotificationRepository { get; }
    IUserAchievementRepository UserAchievementRepository { get; }
    IUserCaseRepository UserCaseRepository { get; }
    IUserEquipmentRepository UserEquipmentRepository { get; }
    IUserExerciseRepository UserExerciseRepository { get; }
    IUserItemRepository UserItemRepository { get; }
    IUserRepository UserRepository { get; }
    IWorkoutRepository WorkoutRepository { get; }
    Task SaveAsync();
}
