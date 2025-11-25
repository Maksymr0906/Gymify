using Gymify.Application.DTOs.Workout;
using Gymify.Application.Services.Implementation;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;
using Moq;
using Xunit;

namespace Gymify.Tests.Services
{
    public class WorkoutServiceTests
    {
        // 1. Оголошуємо моки (імітації) та сам сервіс
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IWorkoutRepository> _mockWorkoutRepo;

        // Додаткові сервіси, які інжектяться в WorkoutService
        private readonly Mock<IUserProfileService> _mockUserProfileService;
        private readonly Mock<IAchievementService> _mockAchievementService;
        private readonly Mock<ICommentService> _mockCommentService;
        private readonly Mock<ICaseService> _mockCaseService;

        private readonly WorkoutService _service;

        public WorkoutServiceTests()
        {
            // 2. Ініціалізуємо моки
            _mockUow = new Mock<IUnitOfWork>();
            _mockWorkoutRepo = new Mock<IWorkoutRepository>();
            _mockUserProfileService = new Mock<IUserProfileService>();
            _mockAchievementService = new Mock<IAchievementService>();
            _mockCommentService = new Mock<ICommentService>();
            _mockCaseService = new Mock<ICaseService>();

            // Налаштовуємо UnitOfWork, щоб він повертав наш фейковий репозиторій
            _mockUow.Setup(u => u.WorkoutRepository).Returns(_mockWorkoutRepo.Object);

            // 3. Створюємо екземпляр сервісу, передаючи йому фейкові залежності
            _service = new WorkoutService(
                _mockUow.Object,
                _mockUserProfileService.Object,
                _mockAchievementService.Object,
                _mockCommentService.Object,
                _mockCaseService.Object
            );
        }

        [Fact]
        public async Task CreateWorkoutAsync_ShouldCreateWorkout_WhenDataIsValid()
        {
            // --- ARRANGE (Підготовка даних) ---
            var userId = Guid.NewGuid();
            var dto = new CreateWorkoutRequestDto
            {
                UserProfileId = userId,
                Name = "Chest Day",
                Description = "Hardcore training",
                Conclusion = "Worked well",
                IsPrivate = false,
            };

            // Налаштовуємо мок: коли викликається CreateAsync, ми нічого не робимо (Task.CompletedTask), бо це void/Task метод
            _mockWorkoutRepo.Setup(r => r.CreateAsync(It.IsAny<Workout>()))
                .ReturnsAsync((Workout w) => w);

            // --- ACT (Виконання дії) ---
            var result = await _service.CreateWorkoutAsync(dto, userId);

            // --- ASSERT (Перевірка результату) ---

            // 1. Перевіряємо, що результат не null і дані співпадають
            Assert.NotNull(result);
            Assert.Equal(dto.Name, result.Name);
            Assert.Equal(userId, result.UserProfileId);

            // 2. ПЕРЕВІРЯЄМО ВИКЛИКИ (Verify)
            // Чи викликався метод репозиторія CreateAsync хоча б один раз?
            _mockWorkoutRepo.Verify(r => r.CreateAsync(It.Is<Workout>(w =>
                w.Name == dto.Name &&
                w.UserProfileId == userId
            )), Times.Once);

            // Чи викликався SaveAsync? (Це критично для бізнес логіки)
            _mockUow.Verify(u => u.SaveAsync(), Times.Once);
        }
    }
}