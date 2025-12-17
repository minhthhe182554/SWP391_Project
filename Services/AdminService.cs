using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SWP391_Project.Models;
using SWP391_Project.Repositories;
using SWP391_Project.ViewModels;
using SWP391_Project.ViewModels.Admin;

namespace SWP391_Project.Services;

    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ILogger<AdminService> _logger;

        public AdminService(IAdminRepository adminRepository, ILogger<AdminService> logger)
        {
            _adminRepository = adminRepository;
            _logger = logger;
        }

        public async Task<User?> GetAdminUserByIdAsync(int userId)
        {
            try
            {
                return await _adminRepository.GetUserByIdAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting admin user by id {UserId}", userId);
                throw;
            }
        }

    public async Task<AdminDashboardVM> GetDashboardMetricsAsync(int days)
        {
            try
            {
            var safeDays = Math.Clamp(days, 1, 30);
            var endDate = DateTime.UtcNow.Date;
            var startDate = endDate.AddDays(-(safeDays - 1));

            var labels = Enumerable.Range(0, safeDays)
                .Select(i => startDate.AddDays(i).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))
                .ToList();

            var newUsers = await _adminRepository.GetNewUsersByDateAsync(startDate, endDate);
            var applications = await _adminRepository.GetApplicationsByDateAsync(startDate, endDate);
            var activeJobs = await _adminRepository.GetActiveJobsByDateAsync(startDate, endDate);
            var activeCompanies = await _adminRepository.GetActiveCompaniesByDateAsync(startDate, endDate);
            var topCategories = await _adminRepository.GetTopJobCategoriesAsync(startDate, endDate, 5);

                return new AdminDashboardVM
                {
                Labels = labels,
                NewUsers = MapSeries(labels, newUsers),
                ActiveJobs = MapSeries(labels, activeJobs),
                NewApplications = MapSeries(labels, applications),
                ActiveCompanies = MapSeries(labels, activeCompanies),
                TopJobCategories = topCategories
                    .Select(kvp => new TopJobCategoryVM
                    {
                        Category = kvp.Key,
                        Applications = kvp.Value
                    })
                    .ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting admin dashboard metrics");
            throw;
        }
    }

    private static List<int> MapSeries(IEnumerable<string> labels, IReadOnlyDictionary<DateTime, int> source)
    {
        var map = source.ToDictionary(k => k.Key.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), v => v.Value);
        var series = new List<int>();
        foreach (var label in labels)
        {
            series.Add(map.TryGetValue(label, out var value) ? value : 0);
        }
        return series;
    }

    public async Task<ManageUsersVM> GetManageUsersAsync(
        int candidateActivePage,
        int candidateBannedPage,
        int companyActivePage,
        int companyBannedPage,
        int pageSize)
    {
        try
        {
            var candidatesActivePaged = await _adminRepository.GetUsersByRoleAndActivePagedAsync(Role.CANDIDATE, true, candidateActivePage, pageSize);
            var candidatesBannedPaged = await _adminRepository.GetUsersByRoleAndActivePagedAsync(Role.CANDIDATE, false, candidateBannedPage, pageSize);
            var companiesActivePaged = await _adminRepository.GetUsersByRoleAndActivePagedAsync(Role.COMPANY, true, companyActivePage, pageSize);
            var companiesBannedPaged = await _adminRepository.GetUsersByRoleAndActivePagedAsync(Role.COMPANY, false, companyBannedPage, pageSize);

            var totalCandidates = await _adminRepository.CountUsersByRoleAsync(Role.CANDIDATE);
            var activeCandidates = await _adminRepository.CountActiveUsersByRoleAsync(Role.CANDIDATE);
            var totalCompanies = await _adminRepository.CountUsersByRoleAsync(Role.COMPANY);
            var activeCompanies = await _adminRepository.CountActiveUsersByRoleAsync(Role.COMPANY);
            var totalJobs = await _adminRepository.CountJobsAsync();
            var activeJobs = await _adminRepository.CountActiveJobsAsync();

            return new ManageUsersVM
            {
                ActiveCandidates = candidatesActivePaged.Users.Select(ToUserItem).ToList(),
                CandidateActivePage = candidateActivePage,
                CandidateActiveTotal = activeCandidates,
                CandidateActiveTotalPages = CalculateTotalPages(activeCandidates, pageSize),
                BannedCandidates = candidatesBannedPaged.Users.Select(ToUserItem).ToList(),
                CandidateBannedPage = candidateBannedPage,
                CandidateBannedTotal = totalCandidates - activeCandidates,
                CandidateBannedTotalPages = CalculateTotalPages(totalCandidates - activeCandidates, pageSize),

                ActiveCompanies = companiesActivePaged.Users.Select(ToUserItem).ToList(),
                CompanyActivePage = companyActivePage,
                CompanyActiveTotal = activeCompanies,
                CompanyActiveTotalPages = CalculateTotalPages(activeCompanies, pageSize),
                BannedCompanies = companiesBannedPaged.Users.Select(ToUserItem).ToList(),
                CompanyBannedPage = companyBannedPage,
                CompanyBannedTotal = totalCompanies - activeCompanies,
                CompanyBannedTotalPages = CalculateTotalPages(totalCompanies - activeCompanies, pageSize),

                PageSize = pageSize,
                TotalCandidatesCount = totalCandidates,
                ActiveCandidatesCount = activeCandidates,
                BannedCandidatesCount = totalCandidates - activeCandidates,
                TotalCompaniesCount = totalCompanies,
                ActiveCompaniesCount = activeCompanies,
                BannedCompaniesCount = totalCompanies - activeCompanies,
                    TotalJobs = totalJobs,
                ActiveJobs = activeJobs
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting manage users data");
            throw;
        }
    }

    public async Task<ManageReportsVM> GetManageReportsAsync(int page, int pageSize)
    {
        try
        {
            var reportsPaged = await _adminRepository.GetJobReportsPagedAsync(page, pageSize);
            var items = reportsPaged.Reports.Select(r => new ReportItemVM
            {
                Id = r.Id,
                JobTitle = r.Job?.Title ?? "N/A",
                CandidateName = r.Candidate?.FullName ?? "N/A",
                Reason = r.Reason,
                Status = r.Status
            }).ToList();

            return new ManageReportsVM
            {
                Reports = items,
                Page = page,
                Total = reportsPaged.Total,
                TotalPages = CalculateTotalPages(reportsPaged.Total, pageSize),
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting manage reports data");
            throw;
        }
    }

    private static int CalculateTotalPages(int total, int pageSize)
    {
        if (pageSize <= 0) return 1;
        return Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
    }

    private static UserItemVM ToUserItem(User user)
    {
        return new UserItemVM
        {
            Id = user.Id,
            Email = user.Email,
            Role = user.Role,
            Active = user.Active
        };
    }

    public async Task<UserDetailVM?> GetUserDetailAsync(int userId)
    {
        try
        {
            var user = await _adminRepository.GetUserWithProfileAsync(userId);
            if (user == null) return null;

            var name = user.Role switch
            {
                Role.CANDIDATE => user.Candidate?.FullName ?? string.Empty,
                Role.COMPANY => user.Company?.Name ?? string.Empty,
                _ => string.Empty
            };
            var roleName = user.Role switch
            {
                Role.ADMIN => "Quản trị",
                Role.CANDIDATE => "Ứng viên",
                Role.COMPANY => "Công ty",
                _ => user.Role.ToString()
            };

            return new UserDetailVM
            {
                Id = user.Id,
                Email = user.Email,
                Name = name,
                Role = user.Role,
                RoleName = roleName,
                Active = user.Active
                };
            }
            catch (Exception ex)
            {
            _logger.LogError(ex, "Error getting user detail for id {UserId}", userId);
                throw;
            }
    }

    public async Task ToggleUserActiveAsync(int userId, bool active)
    {
        try
        {
            await _adminRepository.UpdateUserActiveAsync(userId, active);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling user active status for id {UserId}", userId);
            throw;
        }
    }
}
