namespace NLCNPM.Models;

public record AdminDashboardViewModel(int Users, int Resources, int Approved, int Pending, int Downloads, int Comments, double AverageRating);
public record AdminUserRowViewModel(string Id, string? Email, string DisplayName, bool IsActive, IReadOnlyList<string> Roles);
