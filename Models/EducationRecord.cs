using System;

namespace SWP391_Project.Models;

public class EducationRecord
{
    public int Id { get; set; }

    /// <summary>
    /// Tên trường/bằng cấp/chuyên ngành.
    /// </summary>
    public required string Title { get; set; }

    public required DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    /// <summary>
    /// FK tới Candidate sở hữu record học vấn này.
    /// </summary>
    public int CandidateId { get; set; }

    /// <summary>
    /// Navigation tới Candidate.
    /// </summary>
    public Candidate Candidate { get; set; } = null!;
}
