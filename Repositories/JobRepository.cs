using Microsoft.EntityFrameworkCore; 
using SWP391_Project.Models;

namespace SWP391_Project.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly EzJobDbContext _context;
        public JobRepository(EzJobDbContext context)
        {
            _context = context;
        }

        public async Task<Job?> GetJobWithDetailsAsync(int jobId)
        {
            return await _context.Jobs
                .Include(j => j.Company)
                .ThenInclude(c => c.Location)
                .Include(j => j.Location)
                .Include(j => j.RequiredSkills)
                .Include(j => j.Domains)
                .FirstOrDefaultAsync(j => j.Id == jobId && !j.IsDelete);
        }

        public async Task<List<Job>> GetActiveJobsWithDetailsAsync()
        {
            return await _context.Jobs
                .Where(j => !j.IsDelete)
                .Include(j => j.Company)
                    .ThenInclude(c => c.Location)
                .Include(j => j.Location)
                .Include(j => j.RequiredSkills)
                .Include(j => j.Domains)
                .ToListAsync();
        }

        public async Task<Job?> GetByIdAsync(int id)
        {
            return await _context.Jobs
                .Include(j => j.Company)
                .Include(j => j.Location)
                .Include(j => j.RequiredSkills)
                .Include(j => j.Domains)
                .FirstOrDefaultAsync(j => j.Id == id);
        }

        public async Task AddAsync(Job job)
        {
            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Job>> GetJobsByCompanyIdAsync(int companyId)
        {
            return await _context.Jobs
                .Where(j => j.CompanyId == companyId && !j.IsDelete)
                .Include(j => j.Location)       
                .Include(j => j.Applications)   
                .OrderByDescending(j => j.StartDate) 
                .ToListAsync();
        }
        public async Task UpdateAsync(Job job)
        {
            _context.Jobs.Update(job);
            await _context.SaveChangesAsync();
        }
        public async Task<(List<Job> Jobs, int Total)> SearchAsync(JobSearchQuery query, DateTime now)
        {
            var baseQuery = _context.Jobs
                .Include(j => j.Company)
                    .ThenInclude(c => c.User)
                .Include(j => j.Location)
                .Include(j => j.RequiredSkills)
                .Include(j => j.Domains)
                .Where(j => j.EndDate >= now && !j.IsDelete && j.Company != null && j.Company.User.Active);

            if (!string.IsNullOrWhiteSpace(query.Keyword))
            {
                var keyword = query.Keyword.Trim();
                if (string.Equals(query.KeywordType, "company", StringComparison.OrdinalIgnoreCase))
                {
                    baseQuery = baseQuery.Where(j => j.Company.Name.Contains(keyword));
                }
                else
                {
                    baseQuery = baseQuery.Where(j => j.Title.Contains(keyword));
                }
            }

            if (!string.IsNullOrWhiteSpace(query.City))
            {
                baseQuery = baseQuery.Where(j => j.Location.City == query.City);
            }

            if (!string.IsNullOrWhiteSpace(query.Ward))
            {
                baseQuery = baseQuery.Where(j => j.Location.Ward == query.Ward);
            }

            if (query.DomainIds.Any())
            {
                baseQuery = baseQuery.Where(j => j.Domains.Any(d => query.DomainIds.Contains(d.Id)));
            }

            if (query.MinExperience.HasValue)
            {
                baseQuery = baseQuery.Where(j => j.YearsOfExperience >= query.MinExperience.Value);
            }

            if (query.MaxExperience.HasValue)
            {
                baseQuery = baseQuery.Where(j => j.YearsOfExperience <= query.MaxExperience.Value);
            }

            if (query.MinSalary.HasValue)
            {
                baseQuery = baseQuery.Where(j =>
                    (j.LowerSalaryRange.HasValue && j.LowerSalaryRange.Value >= query.MinSalary.Value) ||
                    (j.HigherSalaryRange.HasValue && j.HigherSalaryRange.Value >= query.MinSalary.Value));
            }

            if (query.MaxSalary.HasValue)
            {
                baseQuery = baseQuery.Where(j =>
                    (j.LowerSalaryRange.HasValue && j.LowerSalaryRange.Value <= query.MaxSalary.Value) ||
                    (j.HigherSalaryRange.HasValue && j.HigherSalaryRange.Value <= query.MaxSalary.Value));
            }

            if (query.JobType.HasValue)
            {
                baseQuery = baseQuery.Where(j => j.Type == query.JobType.Value);
            }

            baseQuery = query.Sort switch
            {
                "salary_desc" => baseQuery.OrderByDescending(j => j.HigherSalaryRange ?? j.LowerSalaryRange ?? 0),
                "salary_asc" => baseQuery.OrderBy(j => j.HigherSalaryRange ?? j.LowerSalaryRange ?? decimal.MaxValue),
                _ => baseQuery.OrderByDescending(j => j.StartDate)
            };

            var total = await baseQuery.CountAsync();
            var currentPage = Math.Max(1, query.Page);
            var pageSize = Math.Max(1, query.PageSize);

            var jobs = await baseQuery
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (jobs, total);
        }

        public async Task<List<Job>> GetRecommendedBySkillsAsync(List<int> skillIds, int minMatchedSkills, int take, DateTime now)
        {
            if (skillIds == null || skillIds.Count == 0 || minMatchedSkills <= 0 || take <= 0)
            {
                return new List<Job>();
            }

            var query = _context.Jobs
                .Include(j => j.Company)
                    .ThenInclude(c => c.User)
                .Include(j => j.Location)
                .Include(j => j.RequiredSkills)
                .Include(j => j.Domains)
                .Where(j => j.EndDate >= now && !j.IsDelete && j.Company != null && j.Company.User.Active)
                .Select(j => new
                {
                    Job = j,
                    MatchCount = j.RequiredSkills.Count(rs => skillIds.Contains(rs.Id))
                })
                .Where(x => x.MatchCount >= minMatchedSkills)
                .OrderByDescending(x => x.MatchCount)
                .ThenByDescending(x => x.Job.StartDate)
                .Take(take);

            return await query.Select(x => x.Job).ToListAsync();
        }
        public IQueryable<Job> GetQueryable()
        {
            return _context.Jobs.AsQueryable();
        }
    }
}

