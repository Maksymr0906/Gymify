using Gymify.Application.DTOs.Comment;
using Gymify.Application.DTOs.UserExercise;
using Gymify.Application.DTOs.Workout;
using Gymify.Application.Services.Implementation;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Entities;
using Gymify.Data.Enums;
using Gymify.Data.Interfaces.Repositories;
using Moq;
using Xunit;

namespace Gymify.Tests.Services
{
    public class WorkoutServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;

        // Репозиторії
        private readonly Mock<IWorkoutRepository> _mockWorkoutRepo;
        private readonly Mock<IUserProfileRepository> _mockUserProfileRepo;
        private readonly Mock<IUserExerciseRepository> _mockUserExerciseRepo;
        private readonly Mock<IItemRepository> _mockItemRepo;

        // Додаткові сервіси
        private readonly Mock<IUserProfileService> _mockUserProfileService;
        private readonly Mock<IAchievementService> _mockAchievementService;
        private readonly Mock<ICommentService> _mockCommentService;
        private readonly Mock<ICaseService> _mockCaseService;

        private readonly WorkoutService _service;

        public WorkoutServiceTests()
        {
            _mockUow = new Mock<IUnitOfWork>();

            // Ініціалізація моків репозиторіїв
            _mockWorkoutRepo = new Mock<IWorkoutRepository>();
            _mockUserProfileRepo = new Mock<IUserProfileRepository>();
            _mockUserExerciseRepo = new Mock<IUserExerciseRepository>();
            _mockItemRepo = new Mock<IItemRepository>();

            // Ініціалізація моків сервісів
            _mockUserProfileService = new Mock<IUserProfileService>();
            _mockAchievementService = new Mock<IAchievementService>();
            _mockCommentService = new Mock<ICommentService>();
            _mockCaseService = new Mock<ICaseService>();

            // Налаштування UnitOfWork
            _mockUow.Setup(u => u.WorkoutRepository).Returns(_mockWorkoutRepo.Object);
            _mockUow.Setup(u => u.UserProfileRepository).Returns(_mockUserProfileRepo.Object);
            _mockUow.Setup(u => u.UserExerciseRepository).Returns(_mockUserExerciseRepo.Object);
            _mockUow.Setup(u => u.ItemRepository).Returns(_mockItemRepo.Object);

            // Ініціалізація тестованого сервісу
            _service = new WorkoutService(
                _mockUow.Object,
                _mockUserProfileService.Object,
                _mockAchievementService.Object,
                _mockCommentService.Object,
                _mockCaseService.Object
            );
        }

        // --- CREATE ---

        [Fact]
        public async Task CreateWorkoutAsync_ShouldCreateWorkout_WhenDataIsValid()
        {
            // ARRANGE
            var userId = Guid.NewGuid();
            var dto = new CreateWorkoutRequestDto
            {
                UserProfileId = userId,
                Name = "Chest Day",
                Description = "Hardcore training",
                Conclusion = "Worked well",
                IsPrivate = false,
            };

            _mockWorkoutRepo.Setup(r => r.CreateAsync(It.IsAny<Workout>()))
                .ReturnsAsync((Workout w) => w);

            // ACT
            var result = await _service.CreateWorkoutAsync(dto, userId);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(dto.Name, result.Name);
            Assert.Equal(userId, result.UserProfileId);

            _mockWorkoutRepo.Verify(r => r.CreateAsync(It.Is<Workout>(w =>
                w.Name == dto.Name &&
                w.UserProfileId == userId
            )), Times.Once);

            _mockUow.Verify(u => u.SaveAsync(), Times.Once);
        }

        // --- COMPLETE ---

        [Fact]
        public async Task CompleteWorkoutAsync_ShouldUpdateStats_AndGiveRewards()
        {
            // ARRANGE
            var workoutId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var request = new CompleteWorkoutRequestDto { WorkoutId = workoutId, UserProfileId = userId, Conclusions = "Done" };

            var workout = new Workout
            {
                Id = workoutId,
                UserProfileId = userId,
                Exercises = new List<UserExercise>
                {
                    new UserExercise { EarnedXP = 50 },
                    new UserExercise { EarnedXP = 50 }
                }
            };

            _mockWorkoutRepo.Setup(r => r.GetByIdWithDetailsAsync(workoutId)).ReturnsAsync(workout);

            // Мокаємо профіль до і після оновлення рівня
            _mockUserProfileRepo.SetupSequence(r => r.GetByIdAsync(userId))
                .ReturnsAsync(new UserProfile { Id = userId, Level = 1 }) // Old level
                .ReturnsAsync(new UserProfile { Id = userId, Level = 2 }); // New level

            _mockAchievementService.Setup(s => s.UpdateUserAchievementsAsync(userId))
                .ReturnsAsync(new List<Achievement>());

            // ACT
            await _service.CompleteWorkoutAsync(request);

            // ASSERT
            Assert.Equal("Done", workout.Conclusion);
            Assert.Equal(100, workout.TotalXP);

            _mockUserProfileService.Verify(s => s.AddXPAsync(userId, 100), Times.Once);
            _mockUserProfileService.Verify(s => s.UpdateStatsAsync(userId, workoutId), Times.Once);
            _mockCaseService.Verify(s => s.GiveRewardByLevelUp(userId, 1), Times.Once); // Level diff = 1
            _mockUow.Verify(u => u.SaveAsync(), Times.Once);
        }

        // --- GET DETAILS ---

        [Fact]
        public async Task GetWorkoutDetailsViewModel_ShouldReturnModel_WhenPublicAndNotOwner()
        {
            // ARRANGE
            var currentUserId = Guid.NewGuid();
            var workoutId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();

            // Воркаут публічний
            var workout = new Workout { Id = workoutId, UserProfileId = ownerId, IsPrivate = false, Name = "Public Workout" };

            _mockWorkoutRepo.Setup(r => r.GetByIdAsync(workoutId)).ReturnsAsync(workout);

            // Мокаємо дані користувачів
            var currentUser = new UserProfile { Equipment = new UserEquipment { AvatarId = Guid.NewGuid() } };
            var ownerUser = new UserProfile { ApplicationUser = new ApplicationUser { UserName = "Owner" } };

            _mockUserProfileRepo.Setup(r => r.GetAllCredentialsAboutUserByIdAsync(currentUserId)).ReturnsAsync(currentUser);
            _mockUserProfileRepo.Setup(r => r.GetAllCredentialsAboutUserByIdAsync(ownerId)).ReturnsAsync(ownerUser);
            _mockItemRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Item { ImageURL = "avatar.png" });

            // Мокаємо вправи та коменти
            _mockUserExerciseRepo.Setup(r => r.GetAllByWorkoutIdAsync(workoutId)).ReturnsAsync(new List<UserExercise>());
            _mockCommentService.Setup(s => s.GetCommentDtos(currentUserId, workoutId, CommentTargetType.Workout))
                .ReturnsAsync(new List<CommentDto>());

            // ACT
            var result = await _service.GetWorkoutDetailsViewModel(currentUserId, workoutId);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(workout.Name, result.Name);
            Assert.False(result.IsOwner); // Не овнер
            Assert.Equal("avatar.png", result.CurrentUserAvatarUrl);
        }

        [Fact]
        public async Task GetWorkoutDetailsViewModel_ShouldThrowKeyNotFound_WhenWorkoutNotExists()
        {
            // ARRANGE
            var workoutId = Guid.NewGuid();
            _mockWorkoutRepo.Setup(r => r.GetByIdAsync(workoutId)).ReturnsAsync((Workout)null);

            // ACT & ASSERT
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.GetWorkoutDetailsViewModel(Guid.NewGuid(), workoutId));
        }

        [Fact]
        public async Task GetWorkoutDetailsViewModel_ShouldThrowUnauthorized_WhenPrivateAndNotOwner()
        {
            // ARRANGE
            var currentUserId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            var workoutId = Guid.NewGuid();

            var workout = new Workout
            {
                Id = workoutId,
                UserProfileId = ownerId,
                IsPrivate = true // ПРИВАТНИЙ
            };

            _mockWorkoutRepo.Setup(r => r.GetByIdAsync(workoutId)).ReturnsAsync(workout);

            // ACT & ASSERT
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.GetWorkoutDetailsViewModel(currentUserId, workoutId));
        }

        // --- UPDATE INFO ---

        [Fact]
        public async Task UpdateWorkoutInfoAsync_ShouldUpdate_WhenUserIsOwner()
        {
            // ARRANGE
            var userId = Guid.NewGuid();
            var workoutId = Guid.NewGuid();
            var dto = new UpdateWorkoutRequestDto
            {
                Id = workoutId,
                Name = "New Name",
                Description = "New Desc",
                IsPrivate = true
            };

            var workout = new Workout { Id = workoutId, UserProfileId = userId, Name = "Old Name" };

            _mockWorkoutRepo.Setup(r => r.GetByIdAsync(workoutId)).ReturnsAsync(workout);

            // ACT
            await _service.UpdateWorkoutInfoAsync(dto, userId);

            // ASSERT
            Assert.Equal("New Name", workout.Name);
            Assert.True(workout.IsPrivate);

            _mockWorkoutRepo.Verify(r => r.UpdateAsync(workout), Times.Once);
            _mockUow.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateWorkoutInfoAsync_ShouldThrow_WhenUserIsNotOwner()
        {
            // ARRANGE
            var userId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            var workoutId = Guid.NewGuid();

            var workout = new Workout { Id = workoutId, UserProfileId = ownerId }; // Власник інший
            _mockWorkoutRepo.Setup(r => r.GetByIdAsync(workoutId)).ReturnsAsync(workout);

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.UpdateWorkoutInfoAsync(new UpdateWorkoutRequestDto { Id = workoutId }, userId));

            Assert.Equal("Access denied", ex.Message);
            _mockWorkoutRepo.Verify(r => r.UpdateAsync(It.IsAny<Workout>()), Times.Never);
        }

        // --- REMOVE ---

        [Fact]
        public async Task RemoveWorkoutAsync_ShouldDelete_WhenUserIsOwner()
        {
            // ARRANGE
            var userId = Guid.NewGuid();
            var workoutId = Guid.NewGuid();
            var workout = new Workout { Id = workoutId, UserProfileId = userId };

            _mockWorkoutRepo.Setup(r => r.GetByIdAsync(workoutId)).ReturnsAsync(workout);

            // ACT
            await _service.RemoveWorkoutAsync(userId, workoutId);

            // ASSERT
            _mockWorkoutRepo.Verify(r => r.DeleteByIdAsync(workoutId), Times.Once);
            _mockUow.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task RemoveWorkoutAsync_ShouldThrow_WhenUserIsNotOwner()
        {
            // ARRANGE
            var userId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            var workoutId = Guid.NewGuid();
            var workout = new Workout { Id = workoutId, UserProfileId = ownerId };

            _mockWorkoutRepo.Setup(r => r.GetByIdAsync(workoutId)).ReturnsAsync(workout);

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.RemoveWorkoutAsync(userId, workoutId));

            Assert.Equal("Access denided", ex.Message);
            _mockWorkoutRepo.Verify(r => r.DeleteByIdAsync(It.IsAny<Guid>()), Times.Never);
        }
    }
}