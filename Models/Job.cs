using System;

namespace SWP391_Project.Models;

public class Job
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public required string Description { get; set; }
    public required string WorkTime { get; set; } // Fulltime / Partime / Hybrid
    public JobType Type { get; set; } = JobType.FULLTIME;
    public required int YearsOfExperience { get; set; }
    public required int VacancyCount { get; set; }
    public decimal? LowerSalaryRange { get; set; }
    public decimal? HigherSalaryRange { get; set; }
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
    public bool IsDelete { get; set; } = false;
    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;
    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;
    public List<Skill> RequiredSkills { get; set; } = new();
    public List<Domain> Domains { get; set; } = new();
    public List<Application> Applications { get; set; } = new();
    public List<Report> Reports { get; set; } = new();
}
