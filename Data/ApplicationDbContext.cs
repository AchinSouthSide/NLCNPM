using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NLCNPM.Models;

namespace NLCNPM.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Resource> Resources => Set<Resource>();
    public DbSet<ResourceVersion> ResourceVersions => Set<ResourceVersion>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Rating> Ratings => Set<Rating>();
    public DbSet<DownloadRecord> Downloads => Set<DownloadRecord>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<ResourceReport> ResourceReports => Set<ResourceReport>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Resource>().HasIndex(x => new { x.Status, x.CreatedAtUtc });
        builder.Entity<Resource>().HasIndex(x => x.Title);
        builder.Entity<Resource>().HasOne(x => x.Owner).WithMany(x => x.Resources)
            .HasForeignKey(x => x.OwnerId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<ResourceVersion>().HasIndex(x => new { x.ResourceId, x.VersionNumber }).IsUnique();
        builder.Entity<DownloadRecord>().HasIndex(x => new { x.ResourceId, x.CreatedAtUtc });
        builder.Entity<ActivityLog>().HasIndex(x => x.CreatedAtUtc);
        builder.Entity<ResourceReport>().HasIndex(x => new { x.Resolved, x.CreatedAtUtc });
        builder.Entity<Rating>().HasIndex(x => new { x.ResourceId, x.UserId }).IsUnique();
        builder.Entity<Rating>().ToTable(t => t.HasCheckConstraint("CK_Rating_Score", "[Score] BETWEEN 1 AND 5"));
        builder.Entity<Comment>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}
