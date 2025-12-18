using SWP391_Project.Models.Enums;

namespace SWP391_Project.ViewModels.Candidate
{
    public class CandidateReportDetailVM
    {
        public int ReportId { get; set; }
        public int JobId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string? CompanyLogo { get; set; }
        public string Reason { get; set; } = string.Empty;
        public ReportStatus Status { get; set; }
        public string StatusCode { get; set; } = string.Empty;
        public string StatusText { get; set; } = string.Empty;
        public bool CanEdit => Status == ReportStatus.PENDING;
    }
}
