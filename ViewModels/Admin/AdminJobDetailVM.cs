using System;
using System.Collections.Generic;
using SWP391_Project.Models;

namespace SWP391_Project.ViewModels.Admin;

public class AdminJobDetailVM
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public int CompanyId { get; set; }
    public int CompanyUserId { get; set; }
    public string CompanyName { get; set; } = string.Empty;

    public string CityName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public JobType JobType { get; set; }
    public string JobTypeName { get; set; } = string.Empty;
    public int YearsOfExperience { get; set; }
    public int VacancyCount { get; set; }

    public decimal? LowerSalaryRange { get; set; }
    public decimal? HigherSalaryRange { get; set; }

    public bool IsDeleted { get; set; }
    public bool IsExpired { get; set; }

    public List<JobReportBriefVM> Reports { get; set; } = new();
}

public class JobReportBriefVM
{
    public int Id { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public ReportStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
}
