using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SWP391_Project.Models;
using SWP391_Project.Repositories;
using SWP391_Project.ViewModels;
using SWP391_Project.ViewModels.Admin;
using SWP391_Project.Constants;

namespace SWP391_Project.Services;

    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly INotificationService _notificationService;
        private readonly ILogger<AdminService> _logger;

        public AdminService(IAdminRepository adminRepository, INotificationService notificationService, ILogger<AdminService> logger)
        {
            _adminRepository = adminRepository;
            _notificationService = notificationService;
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
        int pageSize,
        int? focusUserId = null)
    {
        try
        {
            if (focusUserId.HasValue)
            {
                var info = await _adminRepository.GetUserRoleAndActiveAsync(focusUserId.Value);
                if (info.HasValue)
                {
                    var (role, active) = info.Value;
                    var page = await _adminRepository.GetUserPageByRoleActiveAsync(role, active, focusUserId.Value, pageSize);
                    if (page.HasValue)
                    {
                        if (role == Role.COMPANY)
                        {
                            if (active) companyActivePage = page.Value;
                            else companyBannedPage = page.Value;
                        }
                        else if (role == Role.CANDIDATE)
                        {
                            if (active) candidateActivePage = page.Value;
                            else candidateBannedPage = page.Value;
                        }
                    }
                }
            }

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
                ActiveJobs = activeJobs,
                FocusUserId = focusUserId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting manage users data");
            throw;
        }
    }

    public async Task<ManageReportsVM> GetManageReportsAsync(int page, int pageSize, int? focusReportId = null)
    {
        try
        {
            if (focusReportId.HasValue)
            {
                var focusPage = await _adminRepository.GetReportPageByIdAsync(focusReportId.Value, pageSize);
                if (focusPage.HasValue) page = focusPage.Value;
            }

            var reportsPaged = await _adminRepository.GetJobReportsPagedAsync(page, pageSize);
            var items = reportsPaged.Reports.Select(r => new ReportItemVM
            {
                Id = r.Id,
                JobId = r.JobId,
                JobTitle = r.Job?.Title ?? "N/A",
                CandidateName = r.Candidate?.FullName ?? "N/A",
                CandidateUserId = r.Candidate?.UserId ?? 0,
                Reason = r.Reason,
                Status = r.Status
            }).ToList();

            return new ManageReportsVM
            {
                Reports = items,
                Page = page,
                Total = reportsPaged.Total,
                TotalPages = CalculateTotalPages(reportsPaged.Total, pageSize),
                PageSize = pageSize,
                FocusReportId = focusReportId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting manage reports data");
            throw;
        }
    }

    public async Task<ManageJobsAdminVM> GetManageJobsAsync(int page, int pageSize, string statusFilter, int? focusJobId)
    {
        try
        {
            statusFilter = string.IsNullOrWhiteSpace(statusFilter) ? "all" : statusFilter.Trim().ToLowerInvariant();
            var jobsPaged = await _adminRepository.GetJobsPagedAsync(page, pageSize, statusFilter);
            var reportedJobs = await _adminRepository.GetReportedJobsAsync(statusFilter);

            return new ManageJobsAdminVM
            {
                Jobs = jobsPaged.Jobs.Select(j => new AdminJobItemVM
                {
                    Id = j.Id,
                    Title = j.Title,
                    CompanyName = j.Company?.Name ?? "N/A",
                    CityName = j.Location?.City ?? "N/A",
                    StartDate = j.StartDate,
                    EndDate = j.EndDate,
                    IsDeleted = j.IsDelete
                }).ToList(),
                Page = page,
                Total = jobsPaged.Total,
                TotalPages = CalculateTotalPages(jobsPaged.Total, pageSize),
                PageSize = pageSize,
                FocusJobId = focusJobId,
                StatusFilter = statusFilter,
                ReportedJobs = reportedJobs.Select(j => new ReportedJobItemVM
                {
                    Id = j.Id,
                    Title = j.Title,
                    CompanyName = j.Company?.Name ?? "N/A",
                    CityName = j.Location?.City ?? "N/A",
                    StartDate = j.StartDate,
                    EndDate = j.EndDate,
                    IsDeleted = j.IsDelete
                }).OrderByDescending(x => x.Id)
                  .ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting manage jobs data");
            throw;
        }
    }

    public async Task<AdminJobDetailVM?> GetJobDetailForAdminAsync(int jobId)
    {
        try
        {
            var job = await _adminRepository.GetJobDetailForAdminAsync(jobId);
            if (job == null) return null;

            var isExpired = DateTime.Now > job.EndDate;

            return new AdminJobDetailVM
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                CompanyId = job.CompanyId,
                CompanyUserId = job.Company?.UserId ?? 0,
                CompanyName = job.Company?.Name ?? "N/A",
                CityName = job.Location?.City ?? job.Company?.Location?.City ?? "N/A",
                Address = job.Address ?? job.Company?.Address ?? string.Empty,
                StartDate = job.StartDate,
                EndDate = job.EndDate,
                JobType = job.Type,
                JobTypeName = EnumText.ToVietnamese(job.Type),
                YearsOfExperience = job.YearsOfExperience,
                VacancyCount = job.VacancyCount,
                LowerSalaryRange = job.LowerSalaryRange,
                HigherSalaryRange = job.HigherSalaryRange,
                IsDeleted = job.IsDelete,
                IsExpired = isExpired,
                Reports = job.Reports
                    .OrderByDescending(r => r.Id)
                    .Select(r => new JobReportBriefVM
                    {
                        Id = r.Id,
                        CandidateName = r.Candidate?.FullName ?? "N/A",
                        Reason = r.Reason,
                        Status = r.Status,
                        StatusName = r.Status.ToString()
                    }).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting admin job detail {JobId}", jobId);
            throw;
        }
    }

    public async Task UpdateReportStatusAsync(int reportId, ReportStatus status, string? adminNote = null)
    {
        try
        {
            await _adminRepository.UpdateReportStatusAsync(reportId, status);
            await _notificationService.CreateReportStatusUpdatedAsync(reportId, status, adminNote);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating report status {ReportId}", reportId);
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
                _ => EnumText.ToVietnamese(user.Role)
            };

            var vm = new UserDetailVM
            {
                Id = user.Id,
                Email = user.Email,
                Name = name,
                Role = user.Role,
                RoleName = roleName,
                RoleCode = user.Role.ToString(),
                Active = user.Active
                };

            if (user.Role == Role.COMPANY && user.Company != null)
            {
                vm.JobsCount = await _adminRepository.CountJobsByCompanyIdAsync(user.Company.Id);
                vm.ReportedCount = await _adminRepository.CountReportsAgainstCompanyJobsAsync(user.Company.Id);
            }

            if (user.Role == Role.CANDIDATE && user.Candidate != null)
            {
                vm.FiredReportsCount = await _adminRepository.CountReportsFiredByCandidateAsync(user.Candidate.Id);
                vm.RemainingReport = user.Candidate.RemainingReport;
            }

            return vm;
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
