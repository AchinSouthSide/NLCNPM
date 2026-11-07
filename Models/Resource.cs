using System.ComponentModel.DataAnnotations;

namespace NLCNPM.Models;

public enum ResourceStatus { Pending, Approved, Rejected, Hidden, Archived }

public class Resource
{
    public int Id { get; set; }
    [Required, StringLength(200)] public string Title { get; set; } = string.Empty;
    [Required, StringLength(2000)] public string Description { get; set; } = string.Empty;
    [Required, StringLength(120)] public string OriginalFileName { get; set; } = string.Empty;
    [Required, StringLength(260)] public string StoredFileName { get; set; } = string.Empty;
    [Required, StringLength(100)] public string ContentType { get; set; } = "application/octet-stream";
    public long FileSizeBytes { get; set; }
    public ResourceStatus Status { get; set; } = ResourceStatus.Pending;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public int DownloadCount { get; set; }
    [Required] public string OwnerId { get; set; } = string.Empty;
    public ApplicationUser? Owner { get; set; }
    public ICollection<ResourceVersion> Versions { get; set; } = new List<ResourceVersion>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}
