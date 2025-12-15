using System;
using System.Collections.Generic;
using SWP391_Project.Models;

namespace SWP391_Project.Repositories
{
    public interface IAdminRepository
    {
        Task<User?> GetUserByIdAsync(int userId);
        Task<Dictionary<DateTime, int>> GetNewUsersByDateAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<DateTime, int>> GetApplicationsByDateAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<DateTime, int>> GetActiveJobsByDateAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<DateTime, int>> GetActiveCompaniesByDateAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<string, int>> GetTopJobCategoriesAsync(DateTime startDate, DateTime endDate, int take);

        Task<(List<User> Users, int Total)> GetUsersByRolePagedAsync(Role role, int page, int pageSize);
        Task<(List<User> Users, int Total)> GetUsersByRoleAndActivePagedAsync(Role role, bool active, int page, int pageSize);
        Task<int> CountUsersByRoleAsync(Role role);
        Task<int> CountActiveUsersByRoleAsync(Role role);
        Task<int> CountInactiveUsersByRoleAsync(Role role);

        Task<(List<Report> Reports, int Total)> GetJobReportsPagedAsync(int page, int pageSize);
        Task<int> CountJobsAsync();
        Task<int> CountActiveJobsAsync();

        Task<User?> GetUserWithProfileAsync(int userId);
        Task UpdateUserActiveAsync(int userId, bool active);
    }
}
