/*using Gymify.Application.Services.Implementation;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;
using Moq;
using Xunit;

namespace Gymify.Tests.Services
{
    public class AchievementServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<ICaseService> _mockCaseService;

        // Моки репозиторіїв
        private readonly Mock<IAchievementRepository> _mockAchievementRepo;
        private readonly Mock<IUserProfileRepository> _mockUserProfileRepo;
        private readonly Mock<IUserAchievementRepository> _mockUserAchievementRepo;

        private readonly AchievementService _service;

        public AchievementServiceTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockCaseService = new Mock<ICaseService>();

            _mockAchievementRepo = new Mock<IAchievementRepository>();
            _mockUserProfileRepo = new Mock<IUserProfileRepository>();
            _mockUserAchievementRepo = new Mock<IUserAchievementRepository>();

            // Налаштування UnitOfWork, щоб він повертав наші моки репозиторіїв
            _mockUow.Setup(u => u.AchievementRepository).Returns(_mockAchievementRepo.Object);
            _mockUow.Setup(u => u.UserProfileRepository).Returns(_mockUserProfileRepo.Object);
            _mockUow.Setup(u => u.UserAchievementRepository).Returns(_mockUserAchievementRepo.Object);

            _service = new AchievementService(_mockUow.Object, _mockCaseService.Object);
        }

        [Fact]
        public async Task UpdateUserAchievementsAsync_ShouldCompleteAchievement_WhenConditionIsMet()
        {
            // ARRANGE
            var userId = Guid.NewGuid();

            // 1. Створюємо тестового юзера з потрібним прогресом
            // Використовуємо реальну властивість з UserProfile, наприклад "TotalWorkouts"
            var userProfile = new UserProfile
            {
                Id = userId,
                TotalWorkouts = 10 // Юзер зробив 10 тренувань
            };

            // 2. Створюємо досягнення "Зробити 10 тренувань"
            var achievement = new Achievement
            {
                Id = Guid.NewGuid(),
                TargetProperty = "TotalWorkouts", // Має збігатися з назвою властивості в класі UserProfile
                TargetValue = 10,
                ComparisonType = ">=",
                RewardItemId = Guid.NewGuid()
            };

            // 3. Створюємо зв'язок (юзер ще не виконав це досягнення)
            var userAchievement = new UserAchievement
            {
                UserProfileId = userId,
                AchievementId = achievement.Id,
                IsCompleted = false,
                Progress = 0
            };

            // Додаємо зв'язок до досягнення (як це робить EF Core)
            achievement.UserAchievements = new List<UserAchievement> { userAchievement };

            // Налаштовуємо моки
            _mockAchievementRepo.Setup(r => r.GetAllByUserId(userId))
                .ReturnsAsync(new List<Achievement> { achievement });

            _mockUserProfileRepo.Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(userProfile);

            // ACT
            var result = await _service.UpdateUserAchievementsAsync(userId);

            // ASSERT
            // Перевіряємо, що досягнення позначено як виконане
            Assert.True(userAchievement.IsCompleted);
            Assert.Equal(10, userAchievement.Progress);
            Assert.NotNull(userAchievement.UnlockedAt);

            // Перевіряємо, що досягнення є в списку виконаних
            Assert.Contains(result, a => a.Id == achievement.Id);

            // Перевіряємо, що сервіс нагород викликався
            _mockCaseService.Verify(s => s.GiveRewardByAchievement(userId, achievement.RewardItemId), Times.Once);

            // Перевіряємо збереження в БД
            _mockUow.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAchievementsAsync_ShouldUpdateProgress_ButNotComplete_WhenConditionIsNotMet()
        {
            // ARRANGE
            var userId = Guid.NewGuid();

            // Юзер зробив тільки 5 тренувань
            var userProfile = new UserProfile { Id = userId, TotalWorkouts = 5 };

            // Ціль - 10 тренувань
            var achievement = new Achievement
            {
                Id = Guid.NewGuid(),
                TargetProperty = "TotalWorkouts",
                TargetValue = 10,
                ComparisonType = ">=",
                RewardItemId = Guid.NewGuid()
            };

            var userAchievement = new UserAchievement
            {
                UserProfileId = userId,
                AchievementId = achievement.Id,
                IsCompleted = false,
                Progress = 0
            };
            achievement.UserAchievements = new List<UserAchievement> { userAchievement };

            _mockAchievementRepo.Setup(r => r.GetAllByUserId(userId)).ReturnsAsync(new List<Achievement> { achievement });
            _mockUserProfileRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(userProfile);

            // ACT
            var result = await _service.UpdateUserAchievementsAsync(userId);

            // ASSERT
            Assert.False(userAchievement.IsCompleted); // Не виконано
            Assert.Equal(5, userAchievement.Progress); // Але прогрес оновився до 5
            Assert.Empty(result); // Список виконаних пустий

            // Нагорода НЕ мала видатись
            _mockCaseService.Verify(s => s.GiveRewardByAchievement(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);

            _mockUow.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAchievementsAsync_ShouldNotGiveReward_IfAlreadyCompleted()
        {
            // ARRANGE
            var userId = Guid.NewGuid();
            var userProfile = new UserProfile { Id = userId, TotalWorkouts = 15 };

            var achievement = new Achievement
            {
                Id = Guid.NewGuid(),
                TargetProperty = "TotalWorkouts",
                TargetValue = 10,
                ComparisonType = ">=",
                RewardItemId = Guid.NewGuid()
            };

            var userAchievement = new UserAchievement
            {
                UserProfileId = userId,
                AchievementId = achievement.Id,
                IsCompleted = true, // ВЖЕ ВИКОНАНО
                Progress = 10
            };
            achievement.UserAchievements = new List<UserAchievement> { userAchievement };

            _mockAchievementRepo.Setup(r => r.GetAllByUserId(userId)).ReturnsAsync(new List<Achievement> { achievement });
            _mockUserProfileRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(userProfile);

            // ACT
            await _service.UpdateUserAchievementsAsync(userId);

            // ASSERT
            // Прогрес може оновитись (до 15), але статус не змінюється
            Assert.True(userAchievement.IsCompleted);

            // Нагорода НЕ видається повторно
            _mockCaseService.Verify(s => s.GiveRewardByAchievement(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task SetupUserAchievementsAsync_ShouldCreateUserAchievements_ForAllExistingAchievements()
        {
            // ARRANGE
            var userId = Guid.NewGuid();

            // У нас є 2 глобальних досягнення в базі
            var achievements = new List<Achievement>
            {
                new Achievement { Id = Guid.NewGuid(), Name = "Achiv 1" },
                new Achievement { Id = Guid.NewGuid(), Name = "Achiv 2" }
            };

            _mockAchievementRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(achievements);

            // Setup для CreateAsync (просто повертає Task)
            _mockUserAchievementRepo
                .Setup(r => r.CreateAsync(It.IsAny<UserAchievement>()))
                .ReturnsAsync((UserAchievement ua) => ua);

            // ACT
            await _service.SetupUserAchievementsAsync(userId);

            // ASSERT
            // Перевіряємо, що CreateAsync викликався рівно стільки разів, скільки є досягнень (2 рази)
            _mockUserAchievementRepo.Verify(r => r.CreateAsync(It.Is<UserAchievement>(ua =>
                ua.UserProfileId == userId &&
                !ua.IsCompleted &&
                ua.Progress == 0
            )), Times.Exactly(2));

            _mockUow.Verify(u => u.SaveAsync(), Times.Once);
        }
    }
}*/