using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SWP391_Project.Models;

namespace SWP391_Project.Repositories;

public interface IHomeRepository
{
    Task<HomeJobResult> GetJobsAsync(HomeJobQuery query, DateTime now);
    Task<List<Domain>> GetTopDomainsAsync(DateTime now, int take);
    Task<List<Domain>> GetCompanyDomainOptionsAsync(DateTime now, int take);
    Task<HomeCompanyResult> GetCompaniesAsync(HomeCompanyQuery query, DateTime now);
}

public class HomeRepository : IHomeRepository
{
    private readonly EzJobDbContext _context;

    public HomeRepository(EzJobDbContext context)
    {
        _context = context;
    }

    public async Task<HomeJobResult> GetJobsAsync(HomeJobQuery query, DateTime now)
    {
        var baseQuery = _context.Jobs
            .Include(j => j.Company)
                .ThenInclude(c => c.User)
            .Include(j => j.Location)
            .Include(j => j.RequiredSkills)
            .Include(j => j.Domains)
            .Where(j => j.EndDate >= now && !j.IsDelete && j.Company != null && j.Company.User.Active);

        if (!string.IsNullOrWhiteSpace(query.Location) &&
            !string.Equals(query.Location, "Tất cả", StringComparison.OrdinalIgnoreCase))
        {
            baseQuery = baseQuery.Where(j => j.Location.City == query.Location);
        }

        if (query.SalaryBelowTen)
        {
            baseQuery = baseQuery.Where(j =>
                (j.HigherSalaryRange.HasValue && j.HigherSalaryRange < 10_000_000) ||
                (j.LowerSalaryRange.HasValue && j.LowerSalaryRange < 10_000_000));
        }
        else if (query.SalaryAboveFifty)
        {
            baseQuery = baseQuery.Where(j =>
                (j.LowerSalaryRange.HasValue && j.LowerSalaryRange >= 50_000_000) ||
                (j.HigherSalaryRange.HasValue && j.HigherSalaryRange >= 50_000_000));
        }
        else if (query.MinSalary.HasValue && query.MaxSalary.HasValue)
        {
            var min = query.MinSalary.Value;
            var max = query.MaxSalary.Value;
            baseQuery = baseQuery.Where(j =>
                (j.LowerSalaryRange.HasValue && j.LowerSalaryRange <= max && j.LowerSalaryRange >= min) ||
                (j.HigherSalaryRange.HasValue && j.HigherSalaryRange >= min && j.HigherSalaryRange <= max) ||
                (j.LowerSalaryRange.HasValue && j.HigherSalaryRange.HasValue &&
                 j.LowerSalaryRange <= max && j.HigherSalaryRange >= min));
        }

        if (query.ExperienceBelowOne)
        {
            baseQuery = baseQuery.Where(j => j.YearsOfExperience < 1);
        }
        else if (query.ExperienceAtLeastFive)
        {
            baseQuery = baseQuery.Where(j => j.YearsOfExperience >= 5);
        }
        else if (query.ExactExperience.HasValue)
        {
            baseQuery = baseQuery.Where(j => j.YearsOfExperience == query.ExactExperience.Value);
        }

        if (query.DomainId.HasValue)
        {
            baseQuery = baseQuery.Where(j => j.Domains.Any(d => d.Id == query.DomainId.Value));
        }

        baseQuery = query.Sort switch
        {
            "salary_desc" => baseQuery.OrderByDescending(j => j.HigherSalaryRange ?? j.LowerSalaryRange ?? 0),
            "salary_asc" => baseQuery.OrderBy(j => j.HigherSalaryRange ?? j.LowerSalaryRange ?? decimal.MaxValue),
            _ => baseQuery.OrderByDescending(j => j.StartDate)
        };

        var total = await baseQuery.CountAsync();
        var newToday = await baseQuery.CountAsync(j => j.StartDate.Date == now.Date);
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)query.PageSize));
        var currentPage = Math.Min(Math.Max(1, query.Page), totalPages);

        var jobs = await baseQuery
            .Skip((currentPage - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return new HomeJobResult
        {
            Jobs = jobs,
            Total = total,
            NewToday = newToday,
            CurrentPage = currentPage,
            TotalPages = totalPages
        };
    }

    public async Task<List<Domain>> GetTopDomainsAsync(DateTime now, int take)
    {
        return await _context.Domains
            .Include(d => d.Jobs)
            .Where(d => d.Jobs.Any(j => j.EndDate >= now && !j.IsDelete))
            .Select(d => new
            {
                Domain = d,
                JobCount = d.Jobs.Count(j => j.EndDate >= now && !j.IsDelete)
            })
            .OrderByDescending(x => x.JobCount)
            .Take(take)
            .Select(x => x.Domain)
            .ToListAsync();
    }

    public async Task<List<Domain>> GetCompanyDomainOptionsAsync(DateTime now, int take)
    {
        return await _context.Domains
            .Include(d => d.Jobs)
            .Where(d => d.Jobs.Any(j => j.EndDate >= now && !j.IsDelete && j.Company != null && j.Company.User.Active))
            .Select(d => new
            {
                Domain = d,
                ActiveJobCount = d.Jobs.Count(j => j.EndDate >= now && !j.IsDelete && j.Company != null && j.Company.User.Active)
            })
            .OrderByDescending(x => x.ActiveJobCount)
            .Take(take)
            .Select(x => x.Domain)
            .ToListAsync();
    }

    public async Task<HomeCompanyResult> GetCompaniesAsync(HomeCompanyQuery query, DateTime now)
    {
        var companyBaseQuery = _context.Companies
            .Where(c => c.User.Active && c.Jobs.Any(j => j.EndDate >= now && !j.IsDelete && j.Company != null && j.Company.User.Active));

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var kw = query.Keyword.Trim();
            companyBaseQuery = companyBaseQuery.Where(c => c.Name.Contains(kw));
        }

        if (query.DomainId.HasValue)
        {
            companyBaseQuery = companyBaseQuery.Where(c =>
                c.Jobs.Any(j => j.EndDate >= now && !j.IsDelete && j.Company != null && j.Company.User.Active && j.Domains.Any(d => d.Id == query.DomainId.Value)));
        }

        var totalCompanies = await companyBaseQuery.CountAsync();
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCompanies / (double)query.PageSize));
        var currentPage = Math.Min(Math.Max(1, query.Page), totalPages);

        var pageSlice = await companyBaseQuery
            .Select(c => new
            {
                c.Id,
                ActiveJobCount = c.Jobs.Count(j => j.EndDate >= now && !j.IsDelete && j.Company != null && j.Company.User.Active)
            })
            .OrderByDescending(x => x.ActiveJobCount)
            .ThenBy(x => x.Id)
            .Skip((currentPage - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        var companyIds = pageSlice.Select(x => x.Id).ToList();
        var orderIndex = pageSlice.Select((x, idx) => new { x.Id, idx }).ToDictionary(x => x.Id, x => x.idx);
        var activeJobCountMap = pageSlice.ToDictionary(x => x.Id, x => x.ActiveJobCount);

        var companies = await _context.Companies
            .Include(c => c.Jobs)
                .ThenInclude(j => j.Domains)
            .Where(c => companyIds.Contains(c.Id))
            .ToListAsync();

        companies = companies
            .OrderBy(c => orderIndex.GetValueOrDefault(c.Id, int.MaxValue))
            .ToList();

        return new HomeCompanyResult
        {
            Companies = companies,
            ActiveJobCountMap = activeJobCountMap,
            TotalCompanies = totalCompanies,
            CurrentPage = currentPage,
            TotalPages = totalPages
        };
    }
}

public class HomeJobQuery
{
    public string? Location { get; set; }
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
    public bool SalaryBelowTen { get; set; }
    public bool SalaryAboveFifty { get; set; }
    public bool ExperienceBelowOne { get; set; }
    public bool ExperienceAtLeastFive { get; set; }
    public int? ExactExperience { get; set; }
    public int? DomainId { get; set; }
    public string? Sort { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class HomeJobResult
{
    public List<Job> Jobs { get; set; } = new();
    public int Total { get; set; }
    public int NewToday { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}

public class HomeCompanyQuery
{
    public string? Keyword { get; set; }
    public int? DomainId { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class HomeCompanyResult
{
    public List<Company> Companies { get; set; } = new();
    public Dictionary<int, int> ActiveJobCountMap { get; set; } = new();
    public int TotalCompanies { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}
