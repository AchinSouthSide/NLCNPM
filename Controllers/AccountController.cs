using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NLCNPM.Models;

namespace NLCNPM.Controllers;

public class AccountController(UserManager<ApplicationUser> users, SignInManager<ApplicationUser> signIn) : Controller
{
    [HttpGet] public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = new ApplicationUser { UserName = model.Email, Email = model.Email, DisplayName = model.DisplayName.Trim() };
        var result = await users.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            await users.AddToRoleAsync(user, "Member");
            await signIn.SignInAsync(user, false);
            return RedirectToAction("Index", "Home");
        }
        foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
        return View(model);
    }

    [HttpGet] public IActionResult Login(string? returnUrl = null) { ViewBag.ReturnUrl = returnUrl; return View(new LoginViewModel()); }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(model);
        var user = await users.FindByEmailAsync(model.Email);
        if (user is null || !user.IsActive) { ModelState.AddModelError(string.Empty, "Thông tin đăng nhập không hợp lệ hoặc tài khoản đã bị khóa."); return View(model); }
        var result = await signIn.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);
        if (result.Succeeded) return Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl!) : RedirectToAction("Index", "Home");
        ModelState.AddModelError(string.Empty, "Thông tin đăng nhập không hợp lệ.");
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout() { await signIn.SignOutAsync(); return RedirectToAction("Index", "Home"); }
}
