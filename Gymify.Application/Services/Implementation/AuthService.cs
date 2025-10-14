using Gymify.Application.DTOs.Auth;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Entities;
using Gymify.Data.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Gymify.Application.Services.Implementation;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _unitOfWork = unitOfWork;
    }

    public async Task<IdentityResult> RegisterAsync(RegisterRequestDto dto)
    {
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (result.Succeeded)
        {
            var profile = new UserProfile
            {
                Id = Guid.NewGuid(),
                ApplicationUserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                CurrentXP = 0,
                Level = 1
            };

            await _unitOfWork.UserProfileRepository.CreateAsync(profile);
            await _unitOfWork.SaveAsync();
            await _userManager.AddToRoleAsync(user, "User");
            await _signInManager.SignInAsync(user, false);
        }

        return result;
    }

    public async Task<SignInResult> LoginAsync(LoginRequestDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user == null)
            return SignInResult.Failed;

        return await _signInManager.PasswordSignInAsync(user, dto.Password, dto.RememberMe, false);
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}
