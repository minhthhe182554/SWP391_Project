using SWP391_Project.Models.Enums;

namespace SWP391_Project.Models;

public class Report
{
    public int Id { get; set; }
    public required string Reason { get; set; }
    public int JobId { get; set; }
    public Job Job { get; set; } = null!;
    public int CandidateId { get; set; }
    public Candidate Candidate { get; set; } = null!;
    public ReportStatus Status { get; set; } = ReportStatus.PENDING;
}
