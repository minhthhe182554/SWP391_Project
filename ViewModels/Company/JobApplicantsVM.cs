namespace SWP391_Project.ViewModels.Company
{
    public class JobApplicantsVM
    {
        public int JobId { get; set; }
        public string JobTitle { get; set; } = null!;
        public List<ApplicantDto> Applicants { get; set; } = new();
    }

    public class ApplicantDto
    {
        public int ApplicationId { get; set; }
        public int CandidateId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? CoverLetter { get; set; }
        public string CvUrl { get; set; } = null!;
        public string? AvatarUrl { get; set; }
        public DateTime ApplyDate { get; set; }
        public List<string> PreviewUrls { get; set; } = new();
    }
}
