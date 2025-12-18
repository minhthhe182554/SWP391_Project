namespace SWP391_Project.Models;

public class WorkExperience
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
    public int CandidateId { get; set; }
    public Candidate Candidate { get; set; } = null!;
}
