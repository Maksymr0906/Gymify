using Gymify.Application.DTOs.UserEquipment;
using Gymify.Application.Services.Implementation;
using Gymify.Application.Services.Interfaces;
using Gymify.Application.ViewModels.UserProfile;
using Gymify.Data.Entities;
using Gymify.Data.Enums;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace Gymify.Tests.Services
{
    public class UserProfileServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<ILevelingService> _mockLevelingService;
        private readonly Mock<IUserEquipmentService> _mockEquipmentService;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;

        // Репозиторії
        private readonly Mock<IUserProfileRepository> _mockUserProfileRepo;
        private readonly Mock<IWorkoutRepository> _mockWorkoutRepo;
        private readonly Mock<IUserAchievementRepository> _mockUserAchievementRepo;

        private readonly UserProfileService _service;

        public UserProfileServiceTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockLevelingService = new Mock<ILevelingService>();
            _mockEquipmentService = new Mock<IUserEquipmentService>();

            // UserManager boilerplate
            var store = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            _mockUserProfileRepo = new Mock<IUserProfileRepository>();
            _mockWorkoutRepo = new Mock<IWorkoutRepository>();
            _mockUserAchievementRepo = new Mock<IUserAchievementRepository>();

            _mockUow.Setup(u => u.UserProfileRepository).Returns(_mockUserProfileRepo.Object);
            _mockUow.Setup(u => u.WorkoutRepository).Returns(_mockWorkoutRepo.Object);
            _mockUow.Setup(u => u.UserAchievementRepository).Returns(_mockUserAchievementRepo.Object);

            _service = new UserProfileService(
                _mockUow.Object,
                _mockLevelingService.Object,
                _mockEquipmentService.Object,
                _mockUserManager.Object
            );
        }

        [Fact]
        public async Task AddXPAsync_ShouldIncreaseXp_AndRecalculateLevel()
        {
            // ARRANGE
            var userId = Guid.NewGuid();
            var initialXp = 100;
            var earnedXp = 50;
            var expectedNewLevel = 2;

            var user = new UserProfile { Id = userId, CurrentXP = initialXp, Level = 1 };

            _mockUserProfileRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

            // Мокаємо LevelingService, щоб він повернув новий рівень
            _mockLevelingService.Setup(l => l.CalculateLevel(initialXp + earnedXp))
                .Returns(expectedNewLevel);

            // ACT
            await _service.AddXPAsync(userId, earnedXp);

            // ASSERT
            Assert.Equal(initialXp + earnedXp, user.CurrentXP); // XP має збільшитись
            Assert.Equal(expectedNewLevel, user.Level); // Рівень має оновитись

            _mockUserProfileRepo.Verify(r => r.UpdateAsync(user), Times.Once);
            _mockUow.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateStatsAsync_ShouldUpdateWorkoutStats_AndStreak()
        {
            // ARRANGE
            var userId = Guid.NewGuid();
            var workoutId = Guid.NewGuid();

            var user = new UserProfile
            {
                Id = userId,
                TotalWorkouts = 10,
                WorkoutStreak = 5,
                TotalWeightLifted = 1000
            };

            var workout = new Workout
            {
                Id = workoutId,
                CreatedAt = DateTime.UtcNow, // Сьогодні -> стрік має збільшитись
                Exercises = new List<UserExercise>
                {
                    new UserExercise { Type = ExerciseType.Strength, Weight = 100 },
                    new UserExercise { Type = ExerciseType.Cardio, Duration = TimeSpan.FromMinutes(30) } // 30 хв / 10 = 3 км
                }
            };

            _mockUserProfileRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            _mockWorkoutRepo.Setup(r => r.GetByIdWithDetailsAsync(workoutId)).ReturnsAsync(workout);

            // ACT
            await _service.UpdateStatsAsync(userId, workoutId);

            // ASSERT
            Assert.Equal(11, user.TotalWorkouts); // +1
            Assert.Equal(1100, user.TotalWeightLifted); // 1000 + 100
            Assert.Equal(3, user.TotalKmRunned); // 0 + 3
            Assert.Equal(1, user.StrengthExercisesCompleted);
            Assert.Equal(1, user.CardioExercisesCompleted);

            // Streak logic: якщо сьогодні тренувався -> +1
            Assert.Equal(6, user.WorkoutStreak);

            _mockUow.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateStatsAsync_ShouldResetStreak_IfWorkoutWasNotToday()
        {
            // ARRANGE
            var userId = Guid.NewGuid();
            var workoutId = Guid.NewGuid();

            var user = new UserProfile { Id = userId, WorkoutStreak = 5 };

            // Воркаут був вчора або давно
            var workout = new Workout
            {
                Id = workoutId,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                Exercises = new List<UserExercise>()
            };

            _mockUserProfileRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            _mockWorkoutRepo.Setup(r => r.GetByIdWithDetailsAsync(workoutId)).ReturnsAsync(workout);

            // ACT
            await _service.UpdateStatsAsync(userId, workoutId);

            // ASSERT
            Assert.Equal(0, user.WorkoutStreak); // Стрік має скинутись
        }

        [Fact]
        public async Task GetUserProfileModel_ShouldAggregateDataFromServicesAndRepos()
        {
            // ARRANGE
            var userId = Guid.NewGuid();

            // 1. User Credentials
            var userProfile = new UserProfile
            {
                Id = userId,
                Level = 5,
                ApplicationUserId = Guid.NewGuid(),
                ApplicationUser = new ApplicationUser { UserName = "GymBro" }
            };
            _mockUserProfileRepo.Setup(r => r.GetAllCredentialsAboutUserByIdAsync(userId))
                .ReturnsAsync(userProfile);

            // 2. Equipment
            var equipmentDto = new UserEquipmentDto { TitleText = "Iron Man" };
            _mockEquipmentService.Setup(s => s.GetUserEquipmentAsync(userId))
                .ReturnsAsync(equipmentDto);

            // 3. Achievements (Mocking private method call via Repo)
            // GetCompletedAchivementsOfUser викликає UserAchievementRepository.GetAllByUserId
            _mockUserAchievementRepo.Setup(r => r.GetAllByUserId(userId))
                .ReturnsAsync(new List<UserAchievement>());

            // 4. Workouts (Mocking private method call via Repo)
            _mockWorkoutRepo.Setup(r => r.GetLastWorkouts(userId, 28))
                .ReturnsAsync(new List<Workout>());

            // ACT
            var result = await _service.GetUserProfileModel(userId);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(5, result.Level);
            Assert.Equal("GymBro", result.UserName);
            Assert.Equal("Iron Man", result.Title);
            Assert.NotNull(result.UserEquipmentDto);
            Assert.NotNull(result.Achievements);
            Assert.NotNull(result.Workouts);
        }

        [Fact]
        public async Task UpdateUserNameAsync_ShouldCallUserManager_AndSucceed()
        {
            // ARRANGE
            var userId = Guid.NewGuid();
            var newName = "NewName";
            var appUserId = Guid.NewGuid();

            var userProfile = new UserProfile { Id = userId, ApplicationUserId = appUserId };
            var appUser = new ApplicationUser { Id = appUserId, UserName = "OldName" };

            _mockUserProfileRepo.Setup(r => r.GetAllCredentialsAboutUserByIdAsync(userId))
                .ReturnsAsync(userProfile);

            _mockUserManager.Setup(m => m.FindByIdAsync(appUserId.ToString()))
                .ReturnsAsync(appUser);

            _mockUserManager.Setup(m => m.FindByNameAsync(newName))
                .ReturnsAsync((ApplicationUser)null); // Ім'я вільне

            _mockUserManager.Setup(m => m.SetUserNameAsync(appUser, newName))
                .ReturnsAsync(IdentityResult.Success);

            // ACT
            await _service.UpdateUserNameAsync(userId, newName);

            // ASSERT
            _mockUserManager.Verify(m => m.SetUserNameAsync(appUser, newName), Times.Once);
        }
    }
}