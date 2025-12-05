using System;

namespace SWP391_Project.Models;

public class Application
{
    public int Id { get; set; }
    public int CandidateId { get; set; }
    public Candidate Candidate { get; set; } = null!;
    public int JobId { get; set; }
    public Job Job { get; set; } = null!;
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public required string ResumeUrl { get; set; }
    public string? CoverLetter { get; set; }
    public required DateTime SentDate { get; set; }
}
