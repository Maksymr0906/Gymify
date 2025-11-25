using Gymify.Application.Services.Implementation;
using Xunit;

namespace Gymify.Tests.Services
{
    public class LevelingServiceTests
    {
        private readonly LevelingService _service;

        public LevelingServiceTests()
        {
            // Сервіс не має залежностей, тому просто створюємо його (new)
            _service = new LevelingService();
        }

        // Використовуємо [Theory] для перевірки багатьох сценаріїв одним методом
        [Theory]
        [InlineData(0, 0)]       // 0 XP -> 0 Level
        [InlineData(50, 0)]      // 50 XP -> 0 Level (ще не набрав на 1-й)
        [InlineData(99, 0)]      // 99 XP -> 0 Level (майже)
        [InlineData(100, 1)]     // 100 XP -> 1 Level (Рівно)
        [InlineData(101, 1)]     // 101 XP -> 1 Level
        [InlineData(399, 1)]     // 399 XP -> 1 Level (ще не 2-й)
        [InlineData(400, 2)]     // 400 XP -> 2 Level (2^2 * 100 = 400)
        [InlineData(2500, 5)]    // 2500 XP -> 5 Level (5^2 * 100 = 2500)
        public void CalculateLevel_ShouldReturnCorrectFloorLevel(double xp, int expectedLevel)
        {
            // ACT
            var result = _service.CalculateLevel(xp);

            // ASSERT
            Assert.Equal(expectedLevel, result);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 100)]
        [InlineData(2, 400)]     // 2 * 2 * 100
        [InlineData(3, 900)]     // 3 * 3 * 100
        [InlineData(5, 2500)]    // 5 * 5 * 100
        [InlineData(10, 10000)]  // 10 * 10 * 100
        public void GetTotalXpForLevel_ShouldReturnCorrectRequiredXp(int level, double expectedXp)
        {
            // ACT
            var result = _service.GetTotalXpForLevel(level);

            // ASSERT
            Assert.Equal(expectedXp, result);
        }
    }
}