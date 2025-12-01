/*using Gymify.Application.DTOs.Auth;
using Gymify.Application.Services.Implementation;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;
using System.Security.Claims;

namespace Gymify.Tests.Services
{
    public class AuthServiceTests
    {
        // 1. Мок для UserManager
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;

        // 2. Мок для нашого ФЕЙКОВОГО SignInManager
        private readonly Mock<FakeSignInManager> _mockSignInManager;

        // Інші сервіси
        private readonly Mock<IAchievementService> _mockAchievementService;
        private readonly Mock<IItemService> _mockItemService;
        private readonly Mock<IUserEquipmentService> _mockEquipmentService;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IUserProfileRepository> _mockUserProfileRepo;

        private readonly AuthService _service;

        public AuthServiceTests()
        {
            // --- НАЛАШТУВАННЯ USER MANAGER ---
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                userStore.Object, null, null, null, null, null, null, null, null);

            // --- НАЛАШТУВАННЯ SIGN IN MANAGER ---
            _mockSignInManager = new Mock<FakeSignInManager>();

            // --- ІНШІ СЕРВІСИ ---
            _mockAchievementService = new Mock<IAchievementService>();
            _mockItemService = new Mock<IItemService>();
            _mockEquipmentService = new Mock<IUserEquipmentService>();

            _mockUserProfileRepo = new Mock<IUserProfileRepository>();
            _mockUow = new Mock<IUnitOfWork>();
            _mockUow.Setup(u => u.UserProfileRepository).Returns(_mockUserProfileRepo.Object);

            // --- ІНІЦІАЛІЗАЦІЯ TEST SUBJECT ---
            _service = new AuthService(
                _mockUserManager.Object,
                _mockSignInManager.Object, 
                _mockAchievementService.Object,
                _mockItemService.Object,
                _mockEquipmentService.Object,
                _mockUow.Object
            );
        }

        [Fact]
        public async Task RegisterAsync_ShouldSucceed_WhenDataIsValid()
        {
            // ARRANGE
            var dto = new RegisterRequestDto
            {
                UserName = "TestUser",
                Email = "test@test.com",
                Password = "Password123!"
            };

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), dto.Password))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), "User"))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(x => x.AddClaimsAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<Claim>>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserProfileRepo.Setup(x => x.CreateAsync(It.IsAny<UserProfile>()))
                .ReturnsAsync((UserProfile up) => up);

            _mockSignInManager.Setup(x => x.SignInAsync(It.IsAny<ApplicationUser>(), false, null))
                .Returns(Task.CompletedTask);

            // ACT
            var result = await _service.RegisterAsync(dto);

            // ASSERT
            Assert.True(result.Succeeded);

            _mockSignInManager.Verify(x => x.SignInAsync(It.IsAny<ApplicationUser>(), false, null), Times.Once);

            _mockUow.Verify(x => x.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ShouldSucceed_WhenCredentialsAreCorrect()
        {
            // ARRANGE
            var dto = new LoginRequestDto { Email = "test@test.com", Password = "Pass", RememberMe = true };
            var user = new ApplicationUser { Email = dto.Email };

            _mockUserManager.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(user);

            // Налаштовуємо успішний вхід
            _mockSignInManager.Setup(x => x.PasswordSignInAsync(user, dto.Password, dto.RememberMe, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            // ACT
            var result = await _service.LoginAsync(dto);

            // ASSERT
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task LogoutAsync_ShouldCallSignOut()
        {
            // ACT
            await _service.LogoutAsync();

            // ASSERT
            _mockSignInManager.Verify(x => x.SignOutAsync(), Times.Once);
        }
    }
}*/