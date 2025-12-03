using System;

namespace SWP391_Project.Models;

public class WorkExperience
{
    public int Id { get; set; }

    /// <summary>
    /// Tên công ty/vị trí làm việc.
    /// </summary>
    public required string Name { get; set; }

    public string? Description { get; set; }

    public required DateTime StartDate { get; set; }

    public required DateTime EndDate { get; set; }

    /// <summary>
    /// FK tới Candidate sở hữu kinh nghiệm này.
    /// </summary>
    public int CandidateId { get; set; }

    /// <summary>
    /// Navigation tới Candidate.
    /// </summary>
    public Candidate Candidate { get; set; } = null!;
}
