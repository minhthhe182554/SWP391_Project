using System;

namespace SWP391_Project.Models;

public class Resume
{
    public int Id { get; set; }

    /// <summary>
    /// Tên CV hiển thị cho người dùng (có thể do user đặt lại).
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Đường dẫn file CV (PDF) trên storage.
    /// </summary>
    public required string Url { get; set; }

    /// <summary>
    /// FK tới Candidate sở hữu CV.
    /// </summary>
    public int CandidateId { get; set; }

    /// <summary>
    /// Navigation tới Candidate.
    /// </summary>
    public Candidate Candidate { get; set; } = null!;
}
