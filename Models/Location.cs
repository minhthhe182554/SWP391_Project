namespace SWP391_Project.Models;

public class Location
{
    public int Id { get; set; }
    public required string City { get; set; }
    public required string Ward { get; set; }
    public List<Company> Companies { get; set; } = new();
    public List<Job> Jobs { get; set; } = new();
}
