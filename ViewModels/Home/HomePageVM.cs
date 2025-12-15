using SWP391_Project.Models;
using JobEntity = SWP391_Project.Models.Job;

namespace SWP391_Project.ViewModels.Home;

public class HomePageVM
{
    public List<JobEntity> Jobs { get; set; } = new();
    public List<JobCardVM> JobCards { get; set; } = new();
    public List<CompanyCardVM> CompanyCards { get; set; } = new();
    public List<Domain> CompanyDomainOptions { get; set; } = new();
    public int? SelectedCompanyDomainId { get; set; }
    public int CompanyCurrentPage { get; set; } = 1;
    public int CompanyTotalPages { get; set; } = 1;
    public int CompanyTotalCompanies { get; set; } = 0;
    public int CompanyPageSize { get; set; } = 12;
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
    public int TotalJobs { get; set; } = 0;
    public int MarketActiveJobs { get; set; } = 0;
    public int MarketNewJobsToday { get; set; } = 0;
    public int PageSize { get; set; } = 9;
    
    // Filter options
    public string? SelectedLocation { get; set; }
    public string? SelectedSalaryRange { get; set; }
    public string? SelectedExperience { get; set; }
    public int? SelectedDomainId { get; set; }
    public string? SortOption { get; set; }
    
    // Filter data for display
    public List<string> LocationOptions { get; set; } = new();
    public List<SalaryRangeOption> SalaryRangeOptions { get; set; } = new();
    public List<ExperienceOption> ExperienceOptions { get; set; } = new();
    public List<Domain> TopDomains { get; set; } = new();
    public List<SortOption> SortOptions { get; set; } = new();
}

public class JobCardVM
{
    public JobEntity Job { get; set; } = null!;
    public string CompanyImageUrl { get; set; } = string.Empty;
}

public class CompanyCardVM
{
    public Models.Company Company { get; set; } = null!;
    public string CompanyImageUrl { get; set; } = string.Empty;
    public string? DomainName { get; set; }
    public int ActiveJobCount { get; set; }
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

public class SortOption
{
    public string Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
}
