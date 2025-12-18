using SWP391_Project.Models.Enums;

namespace SWP391_Project.ViewModels.Candidate
{
    public class CandidateReportListVM
    {
        public List<CandidateReportItemVM> Reports { get; set; } = new();
        public ReportStatus? Filter { get; set; }
    }
}
