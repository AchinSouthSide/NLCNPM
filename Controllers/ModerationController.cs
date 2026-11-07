using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLCNPM.Data;
using NLCNPM.Models;

namespace NLCNPM.Controllers;

[Authorize(Roles = "Admin")]
public class ModerationController(ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> Index() => View(await db.Resources.Include(x => x.Owner).OrderBy(x => x.Status).ThenByDescending(x => x.CreatedAtUtc).ToListAsync());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SetStatus(int id, ResourceStatus status)
    {
        if (status is not (ResourceStatus.Approved or ResourceStatus.Rejected or ResourceStatus.Hidden or ResourceStatus.Archived)) return BadRequest();
        var resource = await db.Resources.FindAsync(id);
        if (resource is null) return NotFound();
        resource.Status = status;
        await db.SaveChangesAsync();
        TempData["Notice"] = "Đã cập nhật trạng thái tài liệu.";
        return RedirectToAction(nameof(Index));
    }
}
