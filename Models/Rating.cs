using System.ComponentModel.DataAnnotations;

namespace NLCNPM.Models;

public class Rating
{
    public int Id { get; set; }
    public int ResourceId { get; set; }
    public Resource? Resource { get; set; }
    [Required] public string UserId { get; set; } = string.Empty;
    public int Score { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
