using System.ComponentModel.DataAnnotations;

namespace NLCNPM.Models;

public class Comment
{
    public int Id { get; set; }
    public int ResourceId { get; set; }
    public Resource? Resource { get; set; }
    [Required] public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }
    [Required, StringLength(2000)] public string Body { get; set; } = string.Empty;
    public bool IsHidden { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
