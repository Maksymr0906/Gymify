using Gymify.Application.DTOs.Case;
using Gymify.Application.Services.Implementation;
using Gymify.Data.Entities;
using Gymify.Data.Enums;
using Gymify.Data.Interfaces.Repositories;
using Moq;
using Xunit;

namespace Gymify.Tests.Services
{
    public class CaseServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;

        // Репозиторії
        private readonly Mock<ICaseRepository> _mockCaseRepo;
        private readonly Mock<IUserCaseRepository> _mockUserCaseRepo;
        private readonly Mock<ICaseItemRepository> _mockCaseItemRepo;
        private readonly Mock<IItemRepository> _mockItemRepo;
        private readonly Mock<IUserItemRepository> _mockUserItemRepo;

        private readonly CaseService _service;

        public CaseServiceTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockCaseRepo = new Mock<ICaseRepository>();
            _mockUserCaseRepo = new Mock<IUserCaseRepository>();
            _mockCaseItemRepo = new Mock<ICaseItemRepository>();
            _mockItemRepo = new Mock<IItemRepository>();
            _mockUserItemRepo = new Mock<IUserItemRepository>();

            // Налаштування UoW
            _mockUow.Setup(u => u.CaseRepository).Returns(_mockCaseRepo.Object);
            _mockUow.Setup(u => u.UserCaseRepository).Returns(_mockUserCaseRepo.Object);
            _mockUow.Setup(u => u.CaseItemRepository).Returns(_mockCaseItemRepo.Object);
            _mockUow.Setup(u => u.ItemRepository).Returns(_mockItemRepo.Object);
            _mockUow.Setup(u => u.UserItemRepository).Returns(_mockUserItemRepo.Object);

            _service = new CaseService(_mockUow.Object);
        }

        [Fact]
        public async Task GiveRewardByLevelUp_ShouldGiveRandomCases_EqualToLevelsUp()
        {
            // ARRANGE
            var userId = Guid.NewGuid();
            var levelsUp = 3; // Користувач підняв 3 рівні

            var cases = new List<Case>
            {
                new Case { Id = Guid.NewGuid(), Name = "Common Case" },
                new Case { Id = Guid.NewGuid(), Name = "Rare Case" }
            };

            _mockCaseRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(cases);

            // Налаштуємо CreateAsync, щоб він просто повертав Task
            _mockUserCaseRepo.Setup(r => r.CreateAsync(It.IsAny<UserCase>()))
                .ReturnsAsync((UserCase ue) => ue); // Або ReturnsAsync((UserCase c) => c) якщо повертає об'єкт

            // ACT
            await _service.GiveRewardByLevelUp(userId, levelsUp);

            // ASSERT
            // Перевіряємо, що CreateAsync викликався рівно 3 рази (по одному на рівень)
            _mockUserCaseRepo.Verify(r => r.CreateAsync(It.Is<UserCase>(uc =>
                uc.UserProfileId == userId &&
                cases.Any(c => c.Id == uc.CaseId) // ID має бути одним з існуючих кейсів
            )), Times.Exactly(3));

            _mockUow.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task GiveRewardByAchievement_ShouldGiveSpecificCase()
        {
            // ARRANGE
            var userId = Guid.NewGuid();
            var rewardItemId = Guid.NewGuid(); // ID конкретного кейсу-нагороди

            var rewardCase = new Case { Id = rewardItemId, Name = "Achievement Case" };
            _mockCaseRepo.Setup(r => r.GetByIdAsync(rewardItemId)).ReturnsAsync(rewardCase);

            // ACT
            await _service.GiveRewardByAchievement(userId, rewardItemId);

            // ASSERT
            _mockUserCaseRepo.Verify(r => r.CreateAsync(It.Is<UserCase>(uc =>
                uc.UserProfileId == userId &&
                uc.CaseId == rewardItemId
            )), Times.Once);

            _mockUow.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task OpenCaseAsync_ShouldReturnWinner_AndRemoveCaseFromInventory()
        {
            // ARRANGE
            var userId = Guid.NewGuid();
            var caseId = Guid.NewGuid();

            // 1. Юзер має цей кейс
            var userCase = new UserCase { Id = Guid.NewGuid(), UserProfileId = userId, CaseId = caseId };
            _mockUserCaseRepo.Setup(r => r.GetFirstByUserIdAndCaseIdAsync(userId, caseId))
                .ReturnsAsync(userCase);

            // 2. Кейс містить предмети (зв'язок CaseItem)
            var itemId1 = Guid.NewGuid();
            var itemId2 = Guid.NewGuid();
            var caseItems = new List<CaseItem>
            {
                new CaseItem { CaseId = caseId, ItemId = itemId1 },
                new CaseItem { CaseId = caseId, ItemId = itemId2 }
            };
            _mockCaseItemRepo.Setup(r => r.GetAllByCaseIdAsync(caseId)).ReturnsAsync(caseItems);

            // 3. Самі предмети (Items)
            var items = new List<Item>
            {
                new Item { Id = itemId1, Name = "Sword", Rarity = ItemRarity.Common },
                new Item { Id = itemId2, Name = "Shield", Rarity = ItemRarity.Rare }
            };
            _mockItemRepo.Setup(r => r.GetByListOfIdAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(items);

            // ACT
            var result = await _service.OpenCaseAsync(userId, caseId);

            // ASSERT
            Assert.NotNull(result);
            Assert.NotNull(result.Name); // Переможець визначений
            Assert.Equal(100, result.RouletteStrip.Count()); // Стрічка має 100 елементів

            // Перевіряємо, що переможець записався в інвентар юзера
            _mockUserItemRepo.Verify(r => r.CreateAsync(It.Is<UserItem>(ui =>
                ui.UserProfileId == userId &&
                (ui.ItemId == itemId1 || ui.ItemId == itemId2) // Один з предметів
            )), Times.Once);

            // Перевіряємо, що кейс видалено (використано)
            _mockUserCaseRepo.Verify(r => r.DeleteFirstByUserIdAndCaseIdAsync(userId, caseId), Times.Once);

            _mockUow.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task OpenCaseAsync_ShouldThrowException_WhenUserHasNoCase()
        {
            // ARRANGE
            var userId = Guid.NewGuid();
            var caseId = Guid.NewGuid();

            _mockUserCaseRepo.Setup(r => r.GetFirstByUserIdAndCaseIdAsync(userId, caseId))
                .ReturnsAsync((UserCase)null); // Кейс не знайдено

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.OpenCaseAsync(userId, caseId));

            Assert.Equal("No userCase found", ex.Message);
        }

        [Fact]
        public async Task OpenCaseAsync_ShouldThrowException_WhenCaseHasNoItems()
        {
            // ARRANGE
            var userId = Guid.NewGuid();
            var caseId = Guid.NewGuid();

            _mockUserCaseRepo.Setup(r => r.GetFirstByUserIdAndCaseIdAsync(userId, caseId))
                .ReturnsAsync(new UserCase());

            // Кейс пустий
            _mockCaseItemRepo.Setup(r => r.GetAllByCaseIdAsync(caseId))
                .ReturnsAsync(new List<CaseItem>());

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.OpenCaseAsync(userId, caseId));

            Assert.Equal("Case has no rewards", ex.Message);
        }
    }
}