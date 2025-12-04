using Gymify.Application.DTOs.Auth;
using Gymify.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gymify.Web.Controllers;

public class AuthController : BaseController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet("register")]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var result = await _authService.RegisterAsync(dto);

        if (result.Succeeded)
            return RedirectToAction("Index", "Main");

        foreach (var error in result.Errors)
            ModelState.AddModelError("", error.Description);

        return View(dto);
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto); 
        }

        var result = await _authService.LoginAsync(dto);

        if (result.Succeeded)
            return RedirectToAction("Index", "Main");

        ModelState.AddModelError("", IsUkrainian ? "Пароль або пошта неправильні" : "Password or email are wrong");

        NotifyModelStateErrors();

        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        return RedirectToAction("Index", "Main");
    }
}
