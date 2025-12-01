namespace Gymify.Data.Interfaces.Repositories;

public interface IUnitOfWork
{
    IAchievementRepository AchievementRepository { get; }
    ICaseRepository CaseRepository { get; }
    ICaseItemRepository CaseItemRepository { get; }
    ICommentRepository CommentRepository { get; }
    IExerciseRepository ExerciseRepository { get; }
    IItemRepository ItemRepository { get; }
    IMessageRepository MessageRepository { get; }
    INotificationRepository NotificationRepository { get; }
    IUserEquipmentRepository UserEquipmentRepository { get; }
    IUserExerciseRepository UserExerciseRepository { get; }
    IUserProfileRepository UserProfileRepository { get; }
    IWorkoutRepository WorkoutRepository { get; }
    IUserCaseRepository UserCaseRepository { get; }
    IUserItemRepository UserItemRepository { get; }
    IImageRepository ImageRepository { get; }
    IUserAchievementRepository UserAchievementRepository { get; }
    IUserChatRepository UserChatRepository { get; }
    IChatRepository ChatRepository { get; }
    IFriendInviteRepository FriendInviteRepository { get; }
    IFriendshipRepository FriendshipRepository { get; }
    IMessageReadStatusRepository MessageReadStatusRepository { get; }
    Task SaveAsync();
}
