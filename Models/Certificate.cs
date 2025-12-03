namespace SWP391_Project.Models;

/// <summary>
/// Chứng chỉ của Candidate.
/// Được tách thành entity riêng thay vì List&lt;string&gt; để EF Core map được trong SQL.
/// </summary>
public class Certificate
{
    public int Id { get; set; }

    /// <summary>
    /// Tên chứng chỉ (ví dụ: IELTS 7.0, AWS Certified Developer,...).
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// FK tới Candidate sở hữu chứng chỉ.
    /// </summary>
    public int CandidateId { get; set; }

    /// <summary>
    /// Navigation tới Candidate.
    /// Cho phép include từ Candidate sang Certificates và ngược lại.
    /// </summary>
    public Candidate Candidate { get; set; } = null!;
}

