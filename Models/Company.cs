using System;

namespace SWP391_Project.Models;

public class Company
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public required string Name { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Description { get; set; }
    public int LocationId { get; set; }
    public required string Address { get; set; }
    public Location Location { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public string? Website { get; set; }
    public List<Job> Jobs { get; set; } = new();
}
