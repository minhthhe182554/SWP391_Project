using SWP391_Project.Models.Enums;

namespace SWP391_Project.ViewModels.Candidate
{
    public class CandidateReportItemVM
    {
        public int ReportId { get; set; }
        public int JobId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string? CompanyLogo { get; set; }
        public ReportStatus Status { get; set; }
        public string StatusText { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }
}
