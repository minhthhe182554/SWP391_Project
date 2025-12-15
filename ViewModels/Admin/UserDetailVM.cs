using SWP391_Project.Models;

namespace SWP391_Project.ViewModels.Admin;

public class UserDetailVM
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Role Role { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public bool Active { get; set; }
}

