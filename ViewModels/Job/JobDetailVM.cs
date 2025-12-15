using System.Collections.Generic;

namespace SWP391_Project.ViewModels.Jobs
{
    public class JobDetailVM
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string SalaryText { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Experience { get; set; } = string.Empty;
        public string Deadline { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Skills { get; set; } = new();
        public List<string> Domains { get; set; } = new();
        public string CompanyImageUrl { get; set; } = "/imgs/ic_default_avatar.png";
        public string CompanyName { get; set; } = string.Empty;
        public List<string> CompanyDomains { get; set; } = new();
        public string CompanyAddress { get; set; } = string.Empty;
        public int VacancyCount { get; set; }
        public string JobType { get; set; } = string.Empty;
        public List<SimilarJobCardVM> SimilarJobs { get; set; } = new();
        public bool IsSaved { get; set; }
        public bool HasApplied { get; set; }
    }
}

