using SWP391_Project.Models;

namespace SWP391_Project.Repositories;

public interface INotificationRepository
{
    Task<List<Notification>> GetLatestForCandidateAsync(int candidateId, int take);
    Task<List<Notification>> GetAllForCandidateAsync(int candidateId);
    Task<int> CountUnreadForCandidateAsync(int candidateId);
    Task<Notification?> GetByIdForCandidateAsync(int id, int candidateId);
    Task AddAsync(Notification notification);
    Task MarkReadAsync(int id, int candidateId);
}
