namespace SWP391_Project.Models;

public class Skill
{
    public int Id { get; set; }

    public required string Name { get; set; }
    public List<Candidate> Candidates { get; set; } = new();
    public List<Job> Jobs { get; set; } = new();
}
