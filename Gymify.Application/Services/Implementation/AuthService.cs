using Gymify.Application.DTOs.Auth;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Gymify.Application.Services.Implementation;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IAchievementService _achievementService;
    private readonly IItemService _itemService;
    private readonly IUserEquipmentService _equipmentService;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IAchievementService achievementService,
        IItemService itemService,
        IUserEquipmentService equipmentService,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _achievementService = achievementService;
        _itemService = itemService;
        _equipmentService = equipmentService;
        _unitOfWork = unitOfWork;
    }

    public async Task<IdentityResult> RegisterAsync(RegisterRequestDto dto)
    {
        var user = new ApplicationUser
        {
            UserName = dto.UserName,
            Email = dto.Email
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (result.Succeeded)
        {
            var profile = new UserProfile
            {
                Id = Guid.NewGuid(),
                ApplicationUserId = user.Id,
                CurrentXP = 0,
                Level = 1
            };

            await _unitOfWork.UserProfileRepository.CreateAsync(profile);
            await _unitOfWork.SaveAsync();

            user.UserProfileId = profile.Id;
            await _userManager.UpdateAsync(user);

            await _itemService.SetDefaultUserItemsAsync(profile.Id);

            await _equipmentService.SetDefaultEquipment(profile.Id);

            await _achievementService.SetupUserAchievementsAsync(profile.Id);

            await _userManager.AddToRoleAsync(user, "User");

            var claims = new List<Claim>
            {
                new Claim("UserProfileId", profile.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!)
            };

            await _userManager.AddClaimsAsync(user, claims);
            await _signInManager.SignInAsync(user, isPersistent: false);
        }

        return result;
    }

    public async Task<SignInResult> LoginAsync(LoginRequestDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user == null)
            return SignInResult.Failed;

        var result = await _signInManager.PasswordSignInAsync(user, dto.Password, dto.RememberMe, false);

        return result;
    }


    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}
