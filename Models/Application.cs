using System;

namespace SWP391_Project.Models;

public class Application
{
    public int Id { get; set; }

    /// <summary>
    /// FK tới Candidate đã apply.
    /// </summary>
    public int CandidateId { get; set; }

    /// <summary>
    /// Navigation tới Candidate.
    /// </summary>
    public Candidate Candidate { get; set; } = null!;

    /// <summary>
    /// FK tới Job được apply.
    /// </summary>
    public int JobId { get; set; }

    /// <summary>
    /// Navigation tới Job.
    /// </summary>
    public Job Job { get; set; } = null!;

    /// <summary>
    /// Thông tin snapshot tại thời điểm apply (tránh bị đổi khi Candidate sửa profile).
    /// </summary>
    public required string FullName { get; set; }

    public required string Email { get; set; }

    public required string PhoneNumber { get; set; }

    /// <summary>
    /// Link tới file CV đã chọn (bước 1 của Apply Job).
    /// </summary>
    public required string ResumeUrl { get; set; }

    /// <summary>
    /// Cover letter do Candidate nhập (bước 2 của Apply Job).
    /// </summary>
    public string? CoverLetter { get; set; }

    public required DateTime SentDate { get; set; }
}
