using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLCNPM.Data;
using NLCNPM.Models;

namespace NLCNPM.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController(ApplicationDbContext db, UserManager<ApplicationUser> users) : Controller
{
    public async Task<IActionResult> Index()
    {
        var vm = new AdminDashboardViewModel(
            await users.Users.CountAsync(), await db.Resources.CountAsync(),
            await db.Resources.CountAsync(x => x.Status == ResourceStatus.Approved),
            await db.Resources.CountAsync(x => x.Status == ResourceStatus.Pending),
            await db.Downloads.CountAsync(), await db.Comments.CountAsync(),
            await db.Ratings.Select(x => (double?)x.Score).AverageAsync() ?? 0);
        return View(vm);
    }

    public async Task<IActionResult> Users()
    {
        var result = new List<AdminUserRowViewModel>();
        foreach (var user in await users.Users.OrderBy(x => x.Email).ToListAsync())
            result.Add(new(user.Id, user.Email, user.DisplayName, user.IsActive, (await users.GetRolesAsync(user)).ToList()));
        return View(result);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SetActive(string id, bool active)
    {
        var user = await users.FindByIdAsync(id);
        if (user is null) return NotFound();
        if (user.Id == users.GetUserId(User)) return BadRequest("Không thể khóa tài khoản đang sử dụng.");
        user.IsActive = active;
        await users.UpdateAsync(user);
        TempData["Notice"] = active ? "Đã mở khóa tài khoản." : "Đã khóa tài khoản.";
        return RedirectToAction(nameof(Users));
    }
}
