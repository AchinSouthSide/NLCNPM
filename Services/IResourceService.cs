using NLCNPM.Models;

namespace NLCNPM.Services;

public interface IResourceService
{
    Task<(IReadOnlyList<Resource> Items, int Total)> SearchAsync(string? q, int page, int pageSize, bool approvedOnly);
    Task<Resource?> GetAsync(int id, bool approvedOnly);
    Task<Resource> CreateAsync(Resource resource, Stream file, string extension, CancellationToken ct);
    Task<(Stream Stream, string ContentType, string DownloadName)?> OpenDownloadAsync(int id, CancellationToken ct);
    Task<string?> AddVersionAsync(int id, string userId, Stream file, string originalName, string note, CancellationToken ct);
    Task<bool> AddCommentAsync(int id, string userId, string body, CancellationToken ct);
    Task<bool> RateAsync(int id, string userId, int score, CancellationToken ct);
}
