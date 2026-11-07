using System.ComponentModel.DataAnnotations;

namespace NLCNPM.Models;

public class ResourceVersion
{
    public int Id { get; set; }
    public int ResourceId { get; set; }
    public Resource? Resource { get; set; }
    public int VersionNumber { get; set; }
    [Required, StringLength(260)] public string StoredFileName { get; set; } = string.Empty;
    [Required, StringLength(120)] public string OriginalFileName { get; set; } = string.Empty;
    [StringLength(1000)] public string ChangeNote { get; set; } = string.Empty;
    [Required] public string UpdatedById { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
