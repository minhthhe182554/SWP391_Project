using SWP391_Project.Models;

namespace SWP391_Project.ViewModels.Home;

public class HomePageVM
{
    public List<Job> Jobs { get; set; } = new();
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
    public int TotalJobs { get; set; } = 0;
    public int PageSize { get; set; } = 9;
    
    // Filter options
    public string? SelectedLocation { get; set; }
    public string? SelectedSalaryRange { get; set; }
    public string? SelectedExperience { get; set; }
    public int? SelectedDomainId { get; set; }
    
    // Filter data for display
    public List<string> LocationOptions { get; set; } = new();
    public List<SalaryRangeOption> SalaryRangeOptions { get; set; } = new();
    public List<ExperienceOption> ExperienceOptions { get; set; } = new();
    public List<Domain> TopDomains { get; set; } = new();
}

public class SalaryRangeOption
{
    public string Label { get; set; } = string.Empty;
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
}

public class ExperienceOption
{
    public string Label { get; set; } = string.Empty;
    public int? MinYears { get; set; }
    public int? MaxYears { get; set; }
}
