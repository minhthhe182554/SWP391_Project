using Microsoft.EntityFrameworkCore;
using SWP391_Project.Models;
using SWP391_Project.Models.Enums;

namespace SWP391_Project.Repositories;

public interface IReportRepository
{
    Task<bool> HasCandidateReportedAsync(int candidateId, int jobId);
    Task AddAsync(Report report);
    Task<List<Report>> GetReportsByCandidateAsync(int candidateId, ReportStatus? statusFilter = null);
    Task<Report?> GetReportWithJobAsync(int reportId, int candidateId);
    Task<bool> UpdateReasonAsync(int reportId, int candidateId, string newReason);
    Task<bool> DeleteReportAsync(int reportId, int candidateId);
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

    public async Task<List<Report>> GetReportsByCandidateAsync(int candidateId, ReportStatus? statusFilter = null)
    {
        var query = _context.Reports
            .Include(r => r.Job)
                .ThenInclude(j => j.Company)
            .Where(r => r.CandidateId == candidateId);

        if (statusFilter.HasValue)
        {
            query = query.Where(r => r.Status == statusFilter.Value);
        }

        return await query
            .OrderByDescending(r => r.Id)
            .ToListAsync();
    }

    public async Task<Report?> GetReportWithJobAsync(int reportId, int candidateId)
    {
        return await _context.Reports
            .Include(r => r.Job)
                .ThenInclude(j => j.Company)
            .FirstOrDefaultAsync(r => r.Id == reportId && r.CandidateId == candidateId);
    }

    public async Task<bool> UpdateReasonAsync(int reportId, int candidateId, string newReason)
    {
        var report = await _context.Reports
            .FirstOrDefaultAsync(r => r.Id == reportId && r.CandidateId == candidateId);

        if (report == null || report.Status != ReportStatus.PENDING)
        {
            return false;
        }

        report.Reason = newReason;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteReportAsync(int reportId, int candidateId)
    {
        var report = await _context.Reports
            .FirstOrDefaultAsync(r => r.Id == reportId && r.CandidateId == candidateId);

        if (report == null || report.Status != ReportStatus.PENDING)
        {
            return false;
        }

        _context.Reports.Remove(report);
        await _context.SaveChangesAsync();
        return true;
    }
}
