namespace SWP391_Project.Models;

public class Resume
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public required string Url { get; set; }
    public int CandidateId { get; set; }
    public Candidate Candidate { get; set; } = null!;
}
