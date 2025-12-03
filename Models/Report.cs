using System;

namespace SWP391_Project.Models;

public class Report
{
    public int Id { get; set; }

    /// <summary>
    /// Nội dung report (lý do job không phù hợp, fraud,...).
    /// </summary>
    public required string Content { get; set; }

    /// <summary>
    /// FK tới Job bị report.
    /// </summary>
    public int JobId { get; set; }

    /// <summary>
    /// Navigation tới Job.
    /// </summary>
    public Job Job { get; set; } = null!;

    /// <summary>
    /// FK tới Candidate gửi report.
    /// </summary>
    public int CandidateId { get; set; }

    /// <summary>
    /// Navigation tới Candidate.
    /// </summary>
    public Candidate Candidate { get; set; } = null!;

    /// <summary>
    /// Trạng thái xử lý report (Admin Dashboard - Manage Reports).
    /// </summary>
    public ReportStatus Status { get; set; } = ReportStatus.PENDING;
}
