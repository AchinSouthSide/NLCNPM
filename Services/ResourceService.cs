using Microsoft.EntityFrameworkCore;
using NLCNPM.Data;
using NLCNPM.Models;

namespace NLCNPM.Services;

public class ResourceService(ApplicationDbContext db, IWebHostEnvironment env) : IResourceService
{
    private static readonly HashSet<string> Allowed = new(StringComparer.OrdinalIgnoreCase) { ".pdf", ".docx", ".pptx", ".xlsx", ".txt" };

    public async Task<(IReadOnlyList<Resource> Items, int Total)> SearchAsync(string? q, int page, int pageSize, bool approvedOnly)
    {
        var query = db.Resources.AsNoTracking().Include(x => x.Owner).AsQueryable();
        if (approvedOnly) query = query.Where(x => x.Status == ResourceStatus.Approved);
        if (!string.IsNullOrWhiteSpace(q)) query = query.Where(x => x.Title.Contains(q) || x.Description.Contains(q));
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(x => x.CreatedAtUtc).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public Task<Resource?> GetAsync(int id, bool approvedOnly)
    {
        var query = db.Resources.AsNoTracking().Include(x => x.Owner).Include(x => x.Ratings).Include(x => x.Comments).ThenInclude(x => x.User).AsQueryable();
        if (approvedOnly) query = query.Where(x => x.Status == ResourceStatus.Approved);
        return query.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Resource> CreateAsync(Resource resource, Stream file, string extension, CancellationToken ct)
    {
        if (!Allowed.Contains(extension)) throw new InvalidOperationException("Định dạng tệp không được hỗ trợ.");
        var storage = Path.Combine(env.ContentRootPath, "App_Data", "uploads");
        Directory.CreateDirectory(storage);
        resource.StoredFileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        await using (var output = File.Create(Path.Combine(storage, resource.StoredFileName))) await file.CopyToAsync(output, ct);
        resource.FileSizeBytes = file.Length;
        resource.CreatedAtUtc = DateTime.UtcNow;
        db.Resources.Add(resource);
        await db.SaveChangesAsync(ct);
        db.ResourceVersions.Add(new ResourceVersion { ResourceId = resource.Id, VersionNumber = 1, StoredFileName = resource.StoredFileName, OriginalFileName = resource.OriginalFileName, UpdatedById = resource.OwnerId, ChangeNote = "Phiên bản đầu tiên" });
        await db.SaveChangesAsync(ct);
        return resource;
    }

    public async Task<(Stream Stream, string ContentType, string DownloadName)?> OpenDownloadAsync(int id, CancellationToken ct)
    {
        var resource = await db.Resources.FirstOrDefaultAsync(x => x.Id == id && x.Status == ResourceStatus.Approved, ct);
        if (resource is null) return null;
        var path = Path.Combine(env.ContentRootPath, "App_Data", "uploads", resource.StoredFileName);
        if (!File.Exists(path)) return null;
        resource.DownloadCount++;
        var current = await db.ResourceVersions.Where(v => v.ResourceId == id).OrderByDescending(v => v.VersionNumber).FirstOrDefaultAsync(ct);
        if (current is not null) db.Downloads.Add(new DownloadRecord { ResourceId = id, ResourceVersionId = current.Id, UserId = "anonymous" });
        db.ActivityLogs.Add(new ActivityLog { UserId = "anonymous", Action = "Resource.Download", EntityType = "Resource", EntityId = id.ToString() });
        await db.SaveChangesAsync(ct);
        return (File.OpenRead(path), resource.ContentType, resource.OriginalFileName);
    }

    public async Task<string?> AddVersionAsync(int id, string userId, Stream file, string originalName, string note, CancellationToken ct)
    {
        var resource = await db.Resources.Include(x => x.Versions).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (resource is null || resource.OwnerId != userId) return null;
        var extension = Path.GetExtension(originalName);
        if (!Allowed.Contains(extension)) throw new InvalidOperationException("Định dạng tệp không được hỗ trợ.");
        var stored = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var storage = Path.Combine(env.ContentRootPath, "App_Data", "uploads");
        Directory.CreateDirectory(storage);
        await using (var output = File.Create(Path.Combine(storage, stored))) await file.CopyToAsync(output, ct);
        var version = new ResourceVersion { ResourceId = id, VersionNumber = resource.Versions.Count + 1, StoredFileName = stored, OriginalFileName = Path.GetFileName(originalName), UpdatedById = userId, ChangeNote = note };
        db.ResourceVersions.Add(version);
        resource.StoredFileName = stored;
        resource.OriginalFileName = Path.GetFileName(originalName);
        resource.Status = ResourceStatus.Pending;
        await db.SaveChangesAsync(ct);
        return stored;
    }

    public async Task<bool> AddCommentAsync(int id, string userId, string body, CancellationToken ct)
    {
        if (!await db.Resources.AnyAsync(x => x.Id == id && x.Status == ResourceStatus.Approved, ct)) return false;
        db.Comments.Add(new Comment { ResourceId = id, UserId = userId, Body = body.Trim() });
        db.ActivityLogs.Add(new ActivityLog { UserId = userId, Action = "Comment.Create", EntityType = "Resource", EntityId = id.ToString() });
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> RateAsync(int id, string userId, int score, CancellationToken ct)
    {
        if (score is < 1 or > 5 || !await db.Resources.AnyAsync(x => x.Id == id && x.Status == ResourceStatus.Approved, ct)) return false;
        var rating = await db.Ratings.FirstOrDefaultAsync(x => x.ResourceId == id && x.UserId == userId, ct);
        if (rating is null) db.Ratings.Add(new Rating { ResourceId = id, UserId = userId, Score = score }); else rating.Score = score;
        await db.SaveChangesAsync(ct);
        return true;
    }
}
