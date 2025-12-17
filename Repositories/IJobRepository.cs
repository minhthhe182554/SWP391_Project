using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SWP391_Project.Models;

namespace SWP391_Project.Repositories
{
    public interface IJobRepository
    {
        Task<Job?> GetJobWithDetailsAsync(int jobId);
        Task<List<Job>> GetActiveJobsWithDetailsAsync();
        Task<Job?> GetByIdAsync(int id);
        Task AddAsync(Job job);
        Task<List<Job>> GetJobsByCompanyIdAsync(int companyId);
        Task UpdateAsync(Job job);
        Task<(List<Job> Jobs, int Total)> SearchAsync(JobSearchQuery query, DateTime now);
        Task<List<Job>> GetRecommendedBySkillsAsync(List<int> skillIds, int minMatchedSkills, int take, DateTime now);
    }
}

public class JobSearchQuery
{
    public string? Keyword { get; set; }
    public string KeywordType { get; set; } = "job"; // job | company
    public string? City { get; set; }
    public string? Ward { get; set; }
    public List<int> DomainIds { get; set; } = new();
    public int? MinExperience { get; set; }
    public int? MaxExperience { get; set; }
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
    public JobType? JobType { get; set; }
    public string Sort { get; set; } = "date_desc"; // date_desc | salary_desc | salary_asc
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

