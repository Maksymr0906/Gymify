/*using Gymify.Application.DTOs.UserExercise;
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
        private readonly Mock<IUserExerciseRepository> _mockUserExerciseRepo;
        private readonly Mock<IExerciseRepository> _mockExerciseRepo;

        private readonly UserExerciseService _service;

        public UserExerciseServiceTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockUserExerciseRepo = new Mock<IUserExerciseRepository>();
            _mockExerciseRepo = new Mock<IExerciseRepository>();

            // Налаштування UoW
            _mockUow.Setup(u => u.UserExerciseRepository).Returns(_mockUserExerciseRepo.Object);
            _mockUow.Setup(u => u.ExerciseRepository).Returns(_mockExerciseRepo.Object);

            _service = new UserExerciseService(_mockUow.Object);
        }

        [Fact]
        public async Task AddUserExerciseToWorkoutAsync_ShouldCreateNew_WhenExerciseNotExists()
        {
            // ARRANGE
            var dto = new AddUserExerciseToWorkoutRequestDto { Name = "New Exercise", WorkoutId = Guid.NewGuid() };
            var userId = Guid.NewGuid();

            // Імітуємо, що такої вправи ще немає в базі (довіднику)
            _mockExerciseRepo.Setup(r => r.GetByNameAsync(dto.Name))
                .ReturnsAsync((Exercise)null);

            // Налаштовуємо створення
            _mockExerciseRepo.Setup(r => r.CreateAsync(It.IsAny<Exercise>())).ReturnsAsync((Exercise e) => e);
            _mockUserExerciseRepo.Setup(r => r.CreateAsync(It.IsAny<UserExercise>())).ReturnsAsync((UserExercise ue) => ue);

            // ACT
            await _service.AddUserExerciseToWorkoutAsync(dto, userId);

            // ASSERT
            // Мала створитись нова Exercise (в довіднику)
            _mockExerciseRepo.Verify(r => r.CreateAsync(It.Is<Exercise>(e => e.Name == dto.Name)), Times.Once);
            // І UserExercise (в воркауті)
            _mockUserExerciseRepo.Verify(r => r.CreateAsync(It.IsAny<UserExercise>()), Times.Once);
            _mockUow.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task SyncWorkoutExercisesAsync_ShouldHandle_Delete_Update_Create()
        {
            // ARRANGE
            var workoutId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            // Існуючі вправи в БД (2 штуки)
            var existingId1 = Guid.NewGuid(); // Будемо оновлювати
            var existingId2 = Guid.NewGuid(); // Будемо видаляти

            var existingExercises = new List<UserExercise>
            {
                new UserExercise
                {
                    Id = existingId1,
                    Name = "Ex 1",
                    Sets = 3,
                    Exercise = new Exercise { BaseXP = 10, DifficultyMultiplier = 1 } // Для розрахунку XP
                },
                new UserExercise
                {
                    Id = existingId2,
                    Name = "Ex 2 (To Delete)"
                }
            };

            // DTOs які прийшли з фронта (2 штуки: одна стара оновлена, одна нова)
            var dtos = new List<UserExerciseDto>
            {
                // Оновлення (ID збігається)
                new UserExerciseDto { Id = existingId1, Name = "Ex 1", Sets = 5, Reps = 10 }, 
                // Нова (ID новий, якого немає в базі)
                new UserExerciseDto { Id = Guid.NewGuid(), Name = "Ex 3 (New)", Sets = 3 }
            };

            // Мок отримання існуючих
            _mockUserExerciseRepo.Setup(r => r.GetAllByWorkoutIdAsync(workoutId))
                .ReturnsAsync(existingExercises);

            // Мок пошуку вправи в довіднику (для нової)
            _mockExerciseRepo.Setup(r => r.GetByNameAsync("Ex 3 (New)"))
                .ReturnsAsync(new Exercise { Id = Guid.NewGuid(), Name = "Ex 3 (New)", BaseXP = 10, DifficultyMultiplier = 1 });

            // ACT
            await _service.SyncWorkoutExercisesAsync(workoutId, dtos, userId);

            // ASSERT

            // 1. Перевірка ВИДАЛЕННЯ (existingId2 немає в dtos -> має видалитись)
            _mockUserExerciseRepo.Verify(r => r.DeleteRangeAsync(It.Is<List<UserExercise>>(l =>
                l.Count == 1 && l.First().Id == existingId2
            )), Times.Once);

            // 2. Перевірка ОНОВЛЕННЯ (existingId1)
            _mockUserExerciseRepo.Verify(r => r.UpdateAsync(It.Is<UserExercise>(ue =>
                ue.Id == existingId1 && ue.Sets == 5 // Sets змінились з 3 на 5
            )), Times.Once);

            // 3. Перевірка СТВОРЕННЯ (нова вправа)
            _mockUserExerciseRepo.Verify(r => r.CreateAsync(It.Is<UserExercise>(ue =>
                ue.Name == "Ex 3 (New)" && ue.Sets == 3
            )), Times.Once);

            _mockUow.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllWorkoutExercisesAsync_ShouldReturnMappedDtos()
        {
            // ARRANGE
            var workoutId = Guid.NewGuid();
            var entities = new List<UserExercise>
            {
                new UserExercise { Id = Guid.NewGuid(), Name = "Push Up", Sets = 3, Reps = 15, WorkoutId = workoutId }
            };

            _mockUserExerciseRepo.Setup(r => r.GetAllByWorkoutIdAsync(workoutId))
                .ReturnsAsync(entities);

            // ACT
            var result = await _service.GetAllWorkoutExercisesAsync(workoutId);

            // ASSERT
            Assert.Single(result);
            Assert.Equal("Push Up", result[0].Name);
            Assert.Equal(3, result[0].Sets);
            Assert.Equal(15, result[0].Reps);
        }

        [Fact]
        public void CalculateXp_ShouldCalculateCorrectly()
        {
            // Це статичний метод, тому ми можемо викликати його напряму без моків

            // ARRANGE
            var exercise = new Exercise { BaseXP = 10, DifficultyMultiplier = 2.0 };
            var request = new AddUserExerciseToWorkoutRequestDto
            {
                Sets = 3,
                Reps = 10,
                Weight = 0,
                Duration = 0
            };

            // Expected: 2.0 * 3 * 10 * 1.0 = 60

            // ACT
            var result = UserExerciseService.CalculateXp(request, exercise);

            // ASSERT
            Assert.Equal(60, result);
        }

        [Fact]
        public void CalculateXp_ShouldUseBaseXp_WhenResultIsTooLow()
        {
            // ARRANGE
            var exercise = new Exercise { BaseXP = 50, DifficultyMultiplier = 0.1 };
            var request = new AddUserExerciseToWorkoutRequestDto { Sets = 1, Reps = 1 };
            // Calc: 0.1 * 1 * 1 = 0.1. Це менше за BaseXP (50).

            // ACT
            var result = UserExerciseService.CalculateXp(request, exercise);

            // ASSERT
            Assert.Equal(50, result);
        }
    }
}*/