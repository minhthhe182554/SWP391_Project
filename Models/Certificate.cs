namespace SWP391_Project.Models;

public class Certificate
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int CandidateId { get; set; }
    public Candidate Candidate { get; set; } = null!;
}

