namespace SWP391_Project.ViewModels.Candidate
{
    public class AppliedJobVM
    {
        public int JobId { get; set; }
        public string JobTitle { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public string CompanyLogo { get; set; } = null!;
        public string Location { get; set; } = null!;
        public string SalaryText { get; set; } = null!;

        // Thông tin ứng tuyển riêng
        public DateTime AppliedDate { get; set; } // Ngày nộp
        public string ResumeUrl { get; set; } = null!; // Link CV đã dùng
        public string? CoverLetter { get; set; }
    }
}
