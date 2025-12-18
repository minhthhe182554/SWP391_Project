using Microsoft.EntityFrameworkCore;
using SWP391_Project.Models;

namespace SWP391_Project.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly EzJobDbContext _context;

    public NotificationRepository(EzJobDbContext context)
    {
        _context = context;
    }

    public async Task<List<Notification>> GetLatestForCandidateAsync(int candidateId, int take)
    {
        take = Math.Clamp(take, 1, 50);
        return await _context.Notifications
            .Where(n => n.CandidateId == candidateId)
            .OrderByDescending(n => n.Id)
            .Take(take)
            .ToListAsync();
    }

    public async Task<List<Notification>> GetAllForCandidateAsync(int candidateId)
    {
        return await _context.Notifications
            .Where(n => n.CandidateId == candidateId)
            .OrderByDescending(n => n.Id)
            .ToListAsync();
    }

    public async Task<int> CountUnreadForCandidateAsync(int candidateId)
    {
        return await _context.Notifications.CountAsync(n => n.CandidateId == candidateId && !n.Read);
    }

    public async Task<Notification?> GetByIdForCandidateAsync(int id, int candidateId)
    {
        return await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id && n.CandidateId == candidateId);
    }

    public async Task AddAsync(Notification notification)
    {
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }

    public async Task MarkReadAsync(int id, int candidateId)
    {
        var noti = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id && n.CandidateId == candidateId);
        if (noti == null) return;
        if (!noti.Read)
        {
            noti.Read = true;
            await _context.SaveChangesAsync();
        }
    }
}
