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
        CommentRepository = new CommentRepository(context);
        ExerciseRepository = new ExerciseRepository(context);
        ItemRepository = new ItemRepository(context);
        MessageRepository = new MessageRepository(context);
        NotificationRepository = new NotificationRepository(context);
        UserEquipmentRepository = new UserEquipmentRepository(context);
        UserExerciseRepository = new UserExerciseRepository(context);
        UserRepository = new UserRepository(context);
        WorkoutRepository = new WorkoutRepository(context);
    }

    public UnitOfWork(GymifyDbContext context,
        IAchievementRepository achievementRepository,
        ICaseRepository caseRepository,
        ICommentRepository commentRepository,
        IExerciseRepository exerciseRepository,
        IItemRepository itemRepository,
        IMessageRepository messageRepository,
        INotificationRepository notificationRepository,
        IUserEquipmentRepository userEquipmentRepository,
        IUserExerciseRepository userExerciseRepository,
        IUserRepository userRepository,
        IWorkoutRepository workoutRepository) 
    {
        _context = context;

        AchievementRepository = achievementRepository;
        CaseRepository = caseRepository;
        CommentRepository = commentRepository;
        ExerciseRepository = exerciseRepository;
        ItemRepository = itemRepository;
        MessageRepository = messageRepository;
        NotificationRepository = notificationRepository;
        UserEquipmentRepository = userEquipmentRepository;
        UserExerciseRepository = userExerciseRepository;
        UserRepository = userRepository;
        WorkoutRepository = workoutRepository;
    }

    public IAchievementRepository AchievementRepository { get; set; }
    public ICaseRepository CaseRepository { get; set; }
    public ICommentRepository CommentRepository { get; set; }
    public IExerciseRepository ExerciseRepository { get; set; }
    public IItemRepository ItemRepository { get; set; }
    public IMessageRepository MessageRepository { get; set; }
    public INotificationRepository NotificationRepository { get; set; }
    public IUserEquipmentRepository UserEquipmentRepository { get; set; }
    public IUserExerciseRepository UserExerciseRepository { get; set; }
    public IUserRepository UserRepository { get; set; }
    public IWorkoutRepository WorkoutRepository { get; set; }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}
