using Microsoft.EntityFrameworkCore;
using SWP391_Project.Models;
using SWP391_Project.Models.Enums;

namespace SWP391_Project.Repositories;

    public class AdminRepository : IAdminRepository
    {
        private readonly EzJobDbContext _context;

        public AdminRepository(EzJobDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

    public async Task<Dictionary<DateTime, int>> GetNewUsersByDateAsync(DateTime startDate, DateTime endDate)
    {
        return await Task.FromResult(new Dictionary<DateTime, int>());
    }

    public async Task<Dictionary<DateTime, int>> GetApplicationsByDateAsync(DateTime startDate, DateTime endDate)
        {
        var start = startDate.Date;
        var end = endDate.Date;

        return await _context.Applications
            .Where(a => a.SentDate.Date >= start && a.SentDate.Date <= end)
            .GroupBy(a => a.SentDate.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Date, x => x.Count);
        }

    public async Task<Dictionary<DateTime, int>> GetActiveJobsByDateAsync(DateTime startDate, DateTime endDate)
    {
        var start = startDate.Date;
        var end = endDate.Date;

        var jobs = await _context.Jobs.IgnoreQueryFilters()
            .Where(j => j.StartDate.Date <= end && j.EndDate.Date >= start)
            .Select(j => new { j.StartDate, j.EndDate })
            .ToListAsync();

        var result = new Dictionary<DateTime, int>();
        for (var date = start; date <= end; date = date.AddDays(1))
        {
            var count = jobs.Count(j => j.StartDate.Date <= date && j.EndDate.Date >= date);
            result[date] = count;
        }

        return result;
    }

    public async Task<Dictionary<DateTime, int>> GetActiveCompaniesByDateAsync(DateTime startDate, DateTime endDate)
    {
        var start = startDate.Date;
        var end = endDate.Date;

        var jobs = await _context.Jobs.IgnoreQueryFilters()
            .Where(j => j.StartDate.Date <= end && j.EndDate.Date >= start)
            .Select(j => new { j.CompanyId, j.StartDate, j.EndDate })
            .ToListAsync();

        var result = new Dictionary<DateTime, int>();
        for (var date = start; date <= end; date = date.AddDays(1))
        {
            var count = jobs
                .Where(j => j.StartDate.Date <= date && j.EndDate.Date >= date)
                .Select(j => j.CompanyId)
                .Distinct()
                .Count();
            result[date] = count;
        }

        return result;
    }

    public async Task<Dictionary<string, int>> GetTopJobCategoriesAsync(DateTime startDate, DateTime endDate, int take)
    {
        var start = startDate.Date;
        var end = endDate.Date;

        return await _context.Applications
            .Where(a => a.SentDate.Date >= start && a.SentDate.Date <= end)
            .SelectMany(a => a.Job.Domains.Select(d => new { d.Name }))
            .GroupBy(x => x.Name)
            .Select(g => new { Category = g.Key ?? "Khác", Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(take)
            .ToDictionaryAsync(x => x.Category, x => x.Count);
    }

    public async Task<(List<User> Users, int Total)> GetUsersByRolePagedAsync(Role role, int page, int pageSize)
    {
        var query = _context.Users
            .Where(u => u.Role == role)
            .OrderBy(u => u.Id);

        var total = await query.CountAsync();
        var users = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (users, total);
        }

    public async Task<(List<User> Users, int Total)> GetUsersByRoleAndActivePagedAsync(Role role, bool active, int page, int pageSize)
    {
        var query = _context.Users
            .Where(u => u.Role == role && u.Active == active)
            .OrderBy(u => u.Id);

        var total = await query.CountAsync();
        var users = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (users, total);
    }

    public async Task<int> CountUsersByRoleAsync(Role role)
    {
        return await _context.Users.CountAsync(u => u.Role == role);
    }

    public async Task<int> CountActiveUsersByRoleAsync(Role role)
        {
        return await _context.Users.CountAsync(u => u.Role == role && u.Active);
    }

    public async Task<int> CountInactiveUsersByRoleAsync(Role role)
    {
        return await _context.Users.CountAsync(u => u.Role == role && !u.Active);
        }

    public async Task<(List<Report> Reports, int Total)> GetJobReportsPagedAsync(int page, int pageSize)
    {
        var query = _context.Reports
            .Include(r => r.Job)
            .Include(r => r.Candidate)
            .OrderByDescending(r => r.Id);

        var total = await query.CountAsync();
        var reports = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (reports, total);
    }

    public async Task<int?> GetReportPageByIdAsync(int reportId, int pageSize)
    {
        if (pageSize <= 0) return 1;
        var exists = await _context.Reports.AnyAsync(r => r.Id == reportId);
        if (!exists) return null;

        // ordering: Id desc => index = count(Id > reportId) + 1
        var above = await _context.Reports.CountAsync(r => r.Id > reportId);
        var index = above + 1;
        return (int)Math.Ceiling(index / (double)pageSize);
    }

    public async Task UpdateReportStatusAsync(int reportId, ReportStatus status)
    {
        var report = await _context.Reports.FirstOrDefaultAsync(r => r.Id == reportId);
        if (report == null) return;
        report.Status = status;
        await _context.SaveChangesAsync();
    }

    public async Task<(List<Job> Jobs, int Total)> GetJobsPagedAsync(int page, int pageSize, string statusFilter)
    {
        var now = DateTime.UtcNow.Date;
        IQueryable<Job> query = _context.Jobs.IgnoreQueryFilters()
            .Include(j => j.Company)
            .ThenInclude(c => c.User)
            .Include(j => j.Location);

        query = statusFilter?.ToLowerInvariant() switch
        {
            "active" => query.Where(j => !j.IsDelete && j.EndDate.Date >= now),
            "expired" => query.Where(j => !j.IsDelete && j.EndDate.Date < now),
            "deleted" => query.Where(j => j.IsDelete),
            _ => query
        };

        var ordered = query.OrderByDescending(j => j.Id);
        var total = await ordered.CountAsync();
        var jobs = await ordered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (jobs, total);
    }

    public async Task<List<Job>> GetReportedJobsAsync(string statusFilter)
    {
        var now = DateTime.UtcNow.Date;
        IQueryable<Job> query = _context.Jobs.IgnoreQueryFilters()
            .Include(j => j.Company)
            .ThenInclude(c => c.User)
            .Include(j => j.Location)
            .Include(j => j.Reports)
            .Where(j => j.Reports.Any(r => r.Status == ReportStatus.PENDING || r.Status == ReportStatus.REVIEWING))
            ;

        query = statusFilter?.ToLowerInvariant() switch
        {
            "active" => query.Where(j => !j.IsDelete && j.EndDate.Date >= now),
            "expired" => query.Where(j => !j.IsDelete && j.EndDate.Date < now),
            "deleted" => query.Where(j => j.IsDelete),
            _ => query
        };

        return await query.OrderByDescending(j => j.Id).ToListAsync();
    }

    public async Task<Job?> GetJobDetailForAdminAsync(int jobId)
    {
        return await _context.Jobs.IgnoreQueryFilters()
            .Include(j => j.Company)
            .ThenInclude(c => c.User)
            .Include(j => j.Location)
            .Include(j => j.Reports)
            .ThenInclude(r => r.Candidate)
            .FirstOrDefaultAsync(j => j.Id == jobId);
    }

    public async Task<int> CountJobsAsync()
        {
            return await _context.Jobs.IgnoreQueryFilters().CountAsync();
        }

    public async Task<int> CountActiveJobsAsync()
        {
        var today = DateTime.UtcNow.Date;
        return await _context.Jobs.IgnoreQueryFilters()
            .CountAsync(j => j.StartDate.Date <= today && j.EndDate.Date >= today && !j.IsDelete);
        }

    public async Task<User?> GetUserWithProfileAsync(int userId)
        {
        return await _context.Users
            .Include(u => u.Candidate)
            .Include(u => u.Company)
            .FirstOrDefaultAsync(u => u.Id == userId);
        }

    public async Task<(Role Role, bool Active)?> GetUserRoleAndActiveAsync(int userId)
    {
        var u = await _context.Users
            .Where(x => x.Id == userId)
            .Select(x => new { x.Role, x.Active })
            .FirstOrDefaultAsync();
        if (u == null) return null;
        return (u.Role, u.Active);
    }

    public async Task<int?> GetUserPageByRoleActiveAsync(Role role, bool active, int userId, int pageSize)
    {
        if (pageSize <= 0) return 1;
        var exists = await _context.Users.AnyAsync(u => u.Id == userId && u.Role == role && u.Active == active);
        if (!exists) return null;

        // ordering: Id asc => index = count(Id <= userId) within role+active
        var index = await _context.Users.CountAsync(u => u.Role == role && u.Active == active && u.Id <= userId);
        return (int)Math.Ceiling(index / (double)pageSize);
    }

    public async Task<int> CountJobsByCompanyIdAsync(int companyId)
    {
        return await _context.Jobs.IgnoreQueryFilters()
            .CountAsync(j => j.CompanyId == companyId);
    }

    public async Task<int> CountReportsAgainstCompanyJobsAsync(int companyId)
    {
        // reports filed against jobs posted by this company
        return await _context.Reports
            .CountAsync(r => r.Job.CompanyId == companyId);
    }

    public async Task<int> CountReportsFiredByCandidateAsync(int candidateId)
    {
        return await _context.Reports.CountAsync(r => r.CandidateId == candidateId);
    }

    public async Task<ReportNotificationInfo?> GetReportNotificationInfoAsync(int reportId)
    {
        return await _context.Reports
            .Where(r => r.Id == reportId)
            .Select(r => new ReportNotificationInfo
            {
                CandidateId = r.CandidateId,
                JobId = r.JobId,
                JobTitle = r.Job.Title
            })
            .FirstOrDefaultAsync();
    }

    public async Task UpdateUserActiveAsync(int userId, bool active)
        {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return;
        user.Active = active;
        await _context.SaveChangesAsync();
    }
}
