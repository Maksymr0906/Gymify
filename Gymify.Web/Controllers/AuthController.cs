using Gymify.Application.DTOs.Auth;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Entities;
using Gymify.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace Gymify.Web.Controllers;

public class AuthController : BaseController
{
    private readonly IAuthService _authService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly EmailSenderService _emailSenderService;

    public AuthController(IAuthService authService, UserManager<ApplicationUser> userManager, EmailSenderService emailSenderService)
    {
        _authService = authService;
        _userManager = userManager;
        _emailSenderService = emailSenderService;
    }

    [HttpGet("register")]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        var result = await _authService.RegisterAsync(dto);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var callbackUrl = Url.Action(
                "ConfirmEmail",
                "Auth",
                new { userId = user.Id, token = encodedToken },
                protocol: Request.Scheme);

            await _emailSenderService.SendEmailAsync(dto.Email, "Підтвердження пошти Gymify",
                $"Будь ласка, підтвердіть акаунт, натиснувши <a href='{callbackUrl}'>тут</a>.");

            TempData["QuickSuccess"] = IsUkrainian
                ? "Реєстрація успішна! Перевірте пошту для підтвердження."
                : "Registration successful! Please check your email to confirm.";

            return RedirectToAction("Login");
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError("", error.Description);

        NotifyModelStateErrors();
        return View(dto);
    }

    [HttpGet("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (userId == null || token == null) return RedirectToAction("Index", "Main");

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

        if (result.Succeeded)
        {
            TempData["QuickSuccess"] = IsUkrainian ? "Пошту підтверджено! Увійдіть." : "Email confirmed! Please login.";
            return RedirectToAction("Login");
        }

        return View("Error"); 
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

    [HttpGet("ForgotPassword")]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost("ForgotPassword")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                TempData["QuickSuccess"] = IsUkrainian
                    ? "Якщо така пошта існує, ми відправили інструкції."
                    : "If that email exists, we have sent instructions.";
                return RedirectToAction("Login");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var callbackUrl = Url.Action(
                "ResetPassword",
                "Auth",
                new { token = encodedToken, email = dto.Email },
            protocol: Request.Scheme);

            await _emailSenderService.SendEmailAsync(dto.Email, "Скидання пароля Gymify",
                $"Скиньте пароль за <a href='{callbackUrl}'>цим посиланням</a>.");

            TempData["QuickError"] = IsUkrainian
                    ? "Перевірте пошту для скидання пароля."
                    : "Check your email to reset password.";

            return RedirectToAction("Login");
        }
        return View(dto);
    }

    [HttpGet("ResetPassword")]
    public IActionResult ResetPassword(string token = null, string email = null)
    {
        if (token == null || email == null)
        {
            return RedirectToAction("Login");
        }

        var model = new ResetPasswordDto { Token = token, Email = email };
        return View(model);
    }

    [HttpPost("ResetPassword")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(dto.Token));
        var result = await _userManager.ResetPasswordAsync(user, decodedToken, dto.Password);

        if (result.Succeeded)
        {
            TempData["QuickSuccess"] = IsUkrainian ? "Пароль змінено!" : "Password reset successful!";
            return RedirectToAction("Login");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }
        NotifyModelStateErrors();
        return View(dto);
    }
}
