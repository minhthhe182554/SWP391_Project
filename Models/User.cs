using SWP391_Project.Models.Enums;

namespace SWP391_Project.Models;

public class User
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public bool Active { get; set; } = true;
    public required Role Role { get; set; }
    public Candidate? Candidate { get; set; }
    public Company? Company { get; set; }
}
