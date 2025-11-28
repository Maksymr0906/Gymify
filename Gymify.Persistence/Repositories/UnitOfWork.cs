using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;

namespace Gymify.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly GymifyDbContext _context;

    public UnitOfWork(GymifyDbContext context)
    {
        _context = context;

        AchievementRepository = new AchievementRepository(context);
        CaseRepository = new CaseRepository(context);
        CaseItemRepository = new CaseItemRepository(context);
        CommentRepository = new CommentRepository(context);
        ExerciseRepository = new ExerciseRepository(context);
        ItemRepository = new ItemRepository(context);
        MessageRepository = new MessageRepository(context);
        NotificationRepository = new NotificationRepository(context);
        UserEquipmentRepository = new UserEquipmentRepository(context);
        UserExerciseRepository = new UserExerciseRepository(context);
        UserProfileRepository = new UserProfileRepository(context);
        WorkoutRepository = new WorkoutRepository(context);
        UserCaseRepository = new UserCaseRepository(context);
        UserItemRepository = new UserItemRepository(context);
        ImageRepository = new ImageRepository(context);
        UserAchievementRepository = new UserAchievementRepository(context);

        UserChatRepository = new UserChatRepository(context);
        ChatRepository = new ChatRepository(context);
        FriendshipRepository = new FriendshipRepository(context);
        FriendInviteRepository = new FriendInviteRepository(context);
        MessageReadStatusRepository = new MessageReadStatusRepository(context);
    }

    public UnitOfWork(GymifyDbContext context,
        IAchievementRepository achievementRepository,
        ICaseRepository caseRepository,
        ICaseItemRepository caseItemRepository,
        ICommentRepository commentRepository,
        IExerciseRepository exerciseRepository,
        IItemRepository itemRepository,
        IMessageRepository messageRepository,
        INotificationRepository notificationRepository,
        IUserEquipmentRepository userEquipmentRepository,
        IUserExerciseRepository userExerciseRepository,
        IUserProfileRepository userRepository,
        IWorkoutRepository workoutRepository,
        IUserCaseRepository userCaseRepository,
        IUserItemRepository userItemRepository,
        IImageRepository imageRepository,
        IUserAchievementRepository userAchievementRepository,
        IUserChatRepository userChatRepository,
        IChatRepository chatRepository,
        IFriendInviteRepository friendInviteRepository,
        IFriendshipRepository friendshipRepository,
        IMessageReadStatusRepository messageReadStatusRepository
        ) 
    {
        _context = context;

        AchievementRepository = achievementRepository;
        CaseRepository = caseRepository;
        CaseItemRepository = caseItemRepository;
        CommentRepository = commentRepository;
        ExerciseRepository = exerciseRepository;
        ItemRepository = itemRepository;
        MessageRepository = messageRepository;
        NotificationRepository = notificationRepository;
        UserEquipmentRepository = userEquipmentRepository;
        UserExerciseRepository = userExerciseRepository;
        UserProfileRepository = userRepository;
        WorkoutRepository = workoutRepository;
        UserCaseRepository = userCaseRepository;
        UserItemRepository = userItemRepository;
        ImageRepository = imageRepository;
        UserAchievementRepository = userAchievementRepository;
        UserChatRepository = userChatRepository;
        ChatRepository = chatRepository;
        FriendInviteRepository = friendInviteRepository;
        FriendshipRepository = friendshipRepository;
        MessageReadStatusRepository = messageReadStatusRepository;
    }

    public IAchievementRepository AchievementRepository { get; set; }
    public ICaseRepository CaseRepository { get; set; }
    public ICaseItemRepository CaseItemRepository { get; set; }
    public ICommentRepository CommentRepository { get; set; }
    public IExerciseRepository ExerciseRepository { get; set; }
    public IItemRepository ItemRepository { get; set; }
    public IMessageRepository MessageRepository { get; set; }
    public INotificationRepository NotificationRepository { get; set; }
    public IUserEquipmentRepository UserEquipmentRepository { get; set; }
    public IUserExerciseRepository UserExerciseRepository { get; set; }
    public IUserProfileRepository UserProfileRepository { get; set; }
    public IWorkoutRepository WorkoutRepository { get; set; }
    public IUserCaseRepository UserCaseRepository { get; set; }
    public IUserItemRepository UserItemRepository { get; set; }
    public IImageRepository ImageRepository { get; set; }
    public IUserAchievementRepository UserAchievementRepository { get; set; }

    public IUserChatRepository UserChatRepository { get; set; }
    public IChatRepository ChatRepository { get; set; }
    public IFriendInviteRepository FriendInviteRepository { get; set; }
    public IFriendshipRepository FriendshipRepository { get; set; }
    public IMessageReadStatusRepository MessageReadStatusRepository { get; set; }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}
