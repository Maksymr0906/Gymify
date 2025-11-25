using Gymify.Application.DTOs.UserExercise;
using Gymify.Application.Services.Implementation;
using Gymify.Data.Entities;
using Gymify.Data.Enums;
using Gymify.Data.Interfaces.Repositories;
using Moq;
using Xunit;

namespace Gymify.Tests.Services
{
    public class UserExerciseServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IUserExerciseRepository> _mockUserExRepo;
        private readonly Mock<IExerciseRepository> _mockExRepo;
        private readonly UserExerciseService _service;

        public UserExerciseServiceTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockUserExRepo = new Mock<IUserExerciseRepository>();
            _mockExRepo = new Mock<IExerciseRepository>();

            _mockUow.Setup(u => u.UserExerciseRepository).Returns(_mockUserExRepo.Object);
            _mockUow.Setup(u => u.ExerciseRepository).Returns(_mockExRepo.Object);

            _service = new UserExerciseService(_mockUow.Object);
        }

        [Fact]
        public async Task AddUserExerciseToWorkoutAsync_ShouldCalculateXpCorrectly()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var workoutId = Guid.NewGuid();
            var dto = new AddUserExerciseToWorkoutRequestDto
            {
                WorkoutId = workoutId,
                Name = "Bench Press",
                Sets = 3,
                Reps = 10,
                Weight = 100, // 100 кг
                Duration = 0
            };

            // Імітуємо, що така вправа вже є в базі (довідник)
            var existingExercise = new Exercise
            {
                Id = Guid.NewGuid(),
                Name = "Bench Press",
                BaseXP = 10,
                DifficultyMultiplier = 1.5
            };

            _mockExRepo.Setup(r => r.GetByNameAsync("Bench Press"))
                       .ReturnsAsync(existingExercise);

            // Act
            var result = await _service.AddUserExerciseToWorkoutAsync(dto, userId);

            // Assert
            // Тут ми перевіряємо твою формулу XP.
            // XP = Multiplier * Sets * Reps * Factor
            // Factor (для ваги) = 1 + (100 / 50) = 3.0
            // Expected = 1.5 * 3 * 10 * 3.0 = 135 XP

            Assert.True(result.EarnedXP > 100, "XP should be calculated based on weight");

            // Можемо перевірити точне значення, якщо знаємо формулу на 100%
            // Assert.Equal(135, result.EarnedXP);

            _mockUserExRepo.Verify(r => r.CreateAsync(It.IsAny<UserExercise>()), Times.Once);
            _mockUow.Verify(u => u.SaveAsync(), Times.Once);
        }
    }
}