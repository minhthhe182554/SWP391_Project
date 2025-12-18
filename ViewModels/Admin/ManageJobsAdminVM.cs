using System;
using System.Collections.Generic;

namespace SWP391_Project.ViewModels.Admin;

public class ManageJobsAdminVM
{
    public List<AdminJobItemVM> Jobs { get; set; } = new();
    public int Page { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
    public int Total { get; set; }
    public int PageSize { get; set; } = 10;

    public List<ReportedJobItemVM> ReportedJobs { get; set; } = new();
    public int? FocusJobId { get; set; }
    public string StatusFilter { get; set; } = "all"; // all|active|expired|deleted
}

public class AdminJobItemVM
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string CityName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsDeleted { get; set; }
}

public class ReportedJobItemVM
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string CityName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsDeleted { get; set; }
}
