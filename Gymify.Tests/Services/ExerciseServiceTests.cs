using Gymify.Application.Services.Implementation;
using Gymify.Data.Entities;
using Gymify.Data.Enums;
using Gymify.Data.Interfaces.Repositories;
using Moq;
using Xunit;

namespace Gymify.Tests.Services
{
    public class ExerciseServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IExerciseRepository> _mockExerciseRepo;
        private readonly ExerciseService _service;

        public ExerciseServiceTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockExerciseRepo = new Mock<IExerciseRepository>();

            // Налаштовуємо UoW, щоб він віддавав наш мок репозиторія
            _mockUow.Setup(u => u.ExerciseRepository).Returns(_mockExerciseRepo.Object);

            _service = new ExerciseService(_mockUow.Object);
        }

        [Theory] // Theory дозволяє прогнати один тест з різними вхідними даними
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task FindByNameAsync_ShouldReturnEmpty_WhenNameIsNullOrWhiteSpace(string invalidName)
        {
            // ACT
            var result = await _service.FindByNameAsync(invalidName);

            // ASSERT
            Assert.Empty(result);

            // Переконуємось, що ми навіть не турбували базу даних
            _mockExerciseRepo.Verify(x => x.FindByNameAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task FindByNameAsync_ShouldReturnOnlyApprovedExercises()
        {
            // ARRANGE
            string query = "Press";

            var exercisesFromDb = new List<Exercise>
            {
                new Exercise
                {
                    Id = Guid.NewGuid(),
                    Name = "Bench Press",
                    IsApproved = true,  // ✅ Має потрапити у видачу
                    Type = ExerciseType.Strength
                },
                new Exercise
                {
                    Id = Guid.NewGuid(),
                    Name = "Leg Press",
                    IsApproved = true,  // ✅ Має потрапити у видачу
                    Type = ExerciseType.Strength
                },
                new Exercise
                {
                    Id = Guid.NewGuid(),
                    Name = "Bad Press Form",
                    IsApproved = false, // ❌ НЕ має потрапити (на модерації)
                    Type = ExerciseType.Strength
                }
            };

            _mockExerciseRepo.Setup(r => r.FindByNameAsync(query))
                .ReturnsAsync(exercisesFromDb);

            // ACT
            var result = await _service.FindByNameAsync(query);

            // ASSERT
            Assert.Equal(2, result.Count()); // Очікуємо тільки 2 approved
            Assert.Contains(result, e => e.Name == "Bench Press");
            Assert.Contains(result, e => e.Name == "Leg Press");
            Assert.DoesNotContain(result, e => e.Name == "Bad Press Form");
        }

        [Fact]
        public async Task FindByNameAsync_ShouldMapEntityToDtoCorrectly()
        {
            // ARRANGE
            var exercise = new Exercise
            {
                Id = Guid.NewGuid(),
                Name = "Squat",
                Description = "Leg exercise",
                VideoURL = "http://video.com",
                Type = ExerciseType.Cardio, // Для прикладу
                IsApproved = true
            };

            _mockExerciseRepo.Setup(r => r.FindByNameAsync("Squat"))
                .ReturnsAsync(new List<Exercise> { exercise });

            // ACT
            var result = await _service.FindByNameAsync("Squat");
            var dto = result.First();

            // ASSERT
            Assert.Equal(exercise.Id, dto.Id);
            Assert.Equal(exercise.Name, dto.Name);
            Assert.Equal(exercise.Description, dto.Description);
            Assert.Equal(exercise.VideoURL, dto.VideoURL);
            Assert.Equal((int)exercise.Type, dto.Type);
        }

        [Fact]
        public async Task FindByNameAsync_ShouldReturnEmpty_WhenRepoReturnsNull()
        {
            // ARRANGE
            // Імітуємо ситуацію, коли репозиторій повернув null (хоча краще повертати пустий список, але код має бути готовим)
            _mockExerciseRepo.Setup(r => r.FindByNameAsync("Unknown"))
                .ReturnsAsync((IEnumerable<Exercise>)null);

            // ACT
            var result = await _service.FindByNameAsync("Unknown");

            // ASSERT
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}