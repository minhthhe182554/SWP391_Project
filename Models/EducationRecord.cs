using System;

namespace SWP391_Project.Models;

public class EducationRecord
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int CandidateId { get; set; }
    public Candidate Candidate { get; set; } = null!;
}
