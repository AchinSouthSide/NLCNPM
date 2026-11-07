using Microsoft.AspNetCore.Identity;

namespace NLCNPM.Models;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public ICollection<Resource> Resources { get; set; } = new List<Resource>();
}
