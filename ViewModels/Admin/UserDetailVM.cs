using SWP391_Project.Models;

namespace SWP391_Project.ViewModels.Admin;

public class UserDetailVM
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Role Role { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string RoleCode { get; set; } = string.Empty; // ADMIN/CANDIDATE/COMPANY
    public bool Active { get; set; }

    // Candidate stats
    public int FiredReportsCount { get; set; }
    public int RemainingReport { get; set; }

    // Company stats
    public int JobsCount { get; set; }
    public int ReportedCount { get; set; }
}

