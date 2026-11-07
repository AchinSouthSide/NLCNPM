using System.ComponentModel.DataAnnotations;

namespace NLCNPM.Models;

public class DownloadRecord
{
    public long Id { get; set; }
    public int ResourceId { get; set; }
    public int ResourceVersionId { get; set; }
    [Required] public string UserId { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

public class ActivityLog
{
    public long Id { get; set; }
    public string? UserId { get; set; }
    [Required, StringLength(80)] public string Action { get; set; } = string.Empty;
    [StringLength(80)] public string? EntityType { get; set; }
    public string? EntityId { get; set; }
    [StringLength(1000)] public string? Details { get; set; }
    [StringLength(20)] public string Result { get; set; } = "Success";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

public class ResourceReport
{
    public int Id { get; set; }
    public int ResourceId { get; set; }
    [Required] public string ReporterId { get; set; } = string.Empty;
    [Required, StringLength(1000)] public string Reason { get; set; } = string.Empty;
    public bool Resolved { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
