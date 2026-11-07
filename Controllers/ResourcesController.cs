using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLCNPM.Models;
using NLCNPM.Services;

namespace NLCNPM.Controllers;

public class ResourcesController(IResourceService service) : Controller
{
    private static readonly HashSet<string> Allowed = new(StringComparer.OrdinalIgnoreCase) { ".pdf", ".docx", ".pptx", ".xlsx", ".txt" };

    public async Task<IActionResult> Index(string? q, int page = 1)
    {
        page = Math.Max(1, page);
        var (items, total) = await service.SearchAsync(q, page, 12, !User.IsInRole("Admin"));
        ViewBag.Query = q; ViewBag.Page = page; ViewBag.TotalPages = (int)Math.Ceiling(total / 12d);
        return View(items);
    }

    public async Task<IActionResult> Details(int id)
    {
        var item = await service.GetAsync(id, !User.IsInRole("Admin"));
        return item is null ? NotFound() : View(item);
    }

    [Authorize]
    public IActionResult Create() => View();

    [HttpPost, Authorize, ValidateAntiForgeryToken, RequestSizeLimit(26_214_400)]
    public async Task<IActionResult> Create(string title, string description, IFormFile? file, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(title) || title.Length > 200 || string.IsNullOrWhiteSpace(description) || description.Length > 2000)
            ModelState.AddModelError(string.Empty, "Kiểm tra tiêu đề và mô tả (tối đa 200 và 2000 ký tự).");
        if (file is null || file.Length == 0 || file.Length > 25 * 1024 * 1024) ModelState.AddModelError(nameof(file), "Chọn tệp không quá 25 MB.");
        if (file is not null && !Allowed.Contains(Path.GetExtension(file.FileName))) ModelState.AddModelError(nameof(file), "Chỉ nhận PDF, DOCX, PPTX, XLSX hoặc TXT.");
        if (!ModelState.IsValid) return View();
        try
        {
            var resource = new Resource { Title = title.Trim(), Description = description.Trim(), OriginalFileName = Path.GetFileName(file!.FileName), ContentType = file.ContentType, OwnerId = User.FindFirstValue(ClaimTypes.NameIdentifier)! };
            await service.CreateAsync(resource, file.OpenReadStream(), Path.GetExtension(file.FileName), ct);
            TempData["Notice"] = "Tài liệu đã được gửi và đang chờ kiểm duyệt.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex) { ModelState.AddModelError(nameof(file), ex.Message); return View(); }
    }

    public async Task<IActionResult> Download(int id, CancellationToken ct)
    {
        var result = await service.OpenDownloadAsync(id, ct);
        return result is null ? NotFound() : File(result.Value.Stream, result.Value.ContentType, result.Value.DownloadName);
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> Comment(int id, string body, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(body) || body.Length > 2000) return BadRequest("Bình luận phải có từ 1 đến 2000 ký tự.");
        var ok = await service.AddCommentAsync(id, User.FindFirstValue(ClaimTypes.NameIdentifier)!, body, ct);
        return ok ? RedirectToAction(nameof(Details), new { id }) : NotFound();
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> Rate(int id, int score, CancellationToken ct)
    {
        var ok = await service.RateAsync(id, User.FindFirstValue(ClaimTypes.NameIdentifier)!, score, ct);
        return ok ? RedirectToAction(nameof(Details), new { id }) : BadRequest("Điểm đánh giá phải từ 1 đến 5 và tài liệu phải được duyệt.");
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken, RequestSizeLimit(26_214_400)]
    public async Task<IActionResult> AddVersion(int id, IFormFile? file, string note, CancellationToken ct)
    {
        if (file is null || file.Length == 0 || file.Length > 25 * 1024 * 1024) return BadRequest("Tệp không hợp lệ (tối đa 25 MB).");
        try
        {
            var stored = await service.AddVersionAsync(id, User.FindFirstValue(ClaimTypes.NameIdentifier)!, file.OpenReadStream(), Path.GetFileName(file.FileName), note, ct);
            return stored is null ? Forbid() : RedirectToAction(nameof(Details), new { id });
        }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
    }
}
