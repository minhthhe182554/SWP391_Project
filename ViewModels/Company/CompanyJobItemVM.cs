namespace SWP391_Project.ViewModels.Company
{
    public class CompanyJobItemVM
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string SalaryText { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Experience { get; set; } = string.Empty;
        public string JobType { get; set; } = string.Empty;
        public string CompanyImageUrl { get; set; } = "/imgs/ic_default_avatar.png";
    }
}

