using Microsoft.EntityFrameworkCore;
using SWP391_Project.Models;

namespace SWP391_Project.Repositories;

public interface IReportRepository
{
    Task<bool> HasCandidateReportedAsync(int candidateId, int jobId);
    Task AddAsync(Report report);
}

public class ReportRepository : IReportRepository
{
    private readonly EzJobDbContext _context;

    public ReportRepository(EzJobDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HasCandidateReportedAsync(int candidateId, int jobId)
    {
        return await _context.Reports.AnyAsync(r => r.CandidateId == candidateId && r.JobId == jobId);
    }

    public async Task AddAsync(Report report)
    {
        _context.Reports.Add(report);
        await _context.SaveChangesAsync();
    }
}
