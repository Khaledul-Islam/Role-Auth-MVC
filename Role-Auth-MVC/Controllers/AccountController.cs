using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Role_Auth_MVC.Models.DTOs;
using Role_Auth_MVC.Services.Interfaces;
using System.Security.Claims;
using IAuthenticationService = Role_Auth_MVC.Services.Interfaces.IAuthenticationService;

namespace Role_Auth_MVC.Controllers;

public class AccountController(
    IAuthenticationService authService,
    IUserService userService)
    : Controller
{
    [HttpGet]
    public IActionResult Login(string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginDto model, string returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await authService.AuthenticateAsync(model);

        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email)
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(24)
            });

        await userService.UpdateLastLoginAsync(user.Id);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        // Redirect based on role
        if (user.Roles.Contains("Admin"))
            return RedirectToAction("Index", "Admin");
        if (user.Roles.Contains("Customer"))
            return RedirectToAction("Index", "Customer");
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}