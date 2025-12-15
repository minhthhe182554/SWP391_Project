namespace SWP391_Project.ViewModels.Job
{
    public class SavedJobVM
    {
        public int JobId { get; set; }
        public string Title { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLogo { get; set; }
        public string Location { get; set; }
        public string SalaryRange { get; set; }
        public DateTime Deadline { get; set; }

        public bool IsExpired { get; set; }
        public bool IsExpiringSoon { get; set; } 
    }
}
