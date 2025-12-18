using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using SWP391_Project.Constants;
using SWP391_Project.Models.Enums;
using SWP391_Project.Repositories;
using SWP391_Project.Services.Storage;
using SWP391_Project.ViewModels.Candidate;

namespace SWP391_Project.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly ICandidateRepository _candidateRepository;
        private readonly IStorageService _storageService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<ReportService> _logger;

        public ReportService(
            IReportRepository reportRepository,
            ICandidateRepository candidateRepository,
            IStorageService storageService,
            INotificationService notificationService,
            ILogger<ReportService> logger)
        {
            _reportRepository = reportRepository;
            _candidateRepository = candidateRepository;
            _storageService = storageService;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<CandidateReportListVM> GetCandidateReportsAsync(int userId, ReportStatus? statusFilter)
        {
            try
            {
                var candidate = await _candidateRepository.GetByUserIdAsync(userId);
                if (candidate == null)
                {
                    return new CandidateReportListVM();
                }

                var reports = await _reportRepository.GetReportsByCandidateAsync(candidate.Id, statusFilter);

                var items = reports.Select(r => new CandidateReportItemVM
                {
                    ReportId = r.Id,
                    JobId = r.JobId,
                    JobTitle = r.Job?.Title ?? "Công việc",
                    CompanyName = r.Job?.Company?.Name ?? "Công ty",
                    CompanyLogo = BuildCompanyLogo(r.Job?.Company?.ImageUrl),
                    Status = r.Status,
                    StatusText = EnumText.ToVietnamese(r.Status),
                    Reason = r.Reason
                }).ToList();

                return new CandidateReportListVM
                {
                    Reports = items,
                    Filter = statusFilter
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading candidate reports for user {UserId}", userId);
                return new CandidateReportListVM();
            }
        }

        public async Task<CandidateReportDetailVM?> GetCandidateReportDetailAsync(int userId, int reportId)
        {
            try
            {
                var candidate = await _candidateRepository.GetByUserIdAsync(userId);
                if (candidate == null) return null;

                var report = await _reportRepository.GetReportWithJobAsync(reportId, candidate.Id);
                if (report == null) return null;

                return new CandidateReportDetailVM
                {
                    ReportId = report.Id,
                    JobId = report.JobId,
                    JobTitle = report.Job?.Title ?? "Công việc",
                    CompanyName = report.Job?.Company?.Name ?? "Công ty",
                    Reason = report.Reason,
                    Status = report.Status,
                    StatusCode = report.Status.ToString(),
                    StatusText = EnumText.ToVietnamese(report.Status),
                    CompanyLogo = BuildCompanyLogo(report.Job?.Company?.ImageUrl)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading report detail {ReportId} for user {UserId}", reportId, userId);
                return null;
            }
        }

        public async Task<(bool Success, string Message)> UpdateCandidateReportAsync(int userId, int reportId, string newReason)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(newReason))
                {
                    return (false, "Lý do không được để trống");
                }

                if (newReason.Length > 500)
                {
                    return (false, "Lý do không được vượt quá 500 ký tự");
                }

                var candidate = await _candidateRepository.GetByUserIdAsync(userId);
                if (candidate == null)
                {
                    return (false, "Không tìm thấy ứng viên");
                }

                var updated = await _reportRepository.UpdateReasonAsync(reportId, candidate.Id, newReason.Trim());
                if (!updated)
                {
                    return (false, "Chỉ chỉnh sửa được báo cáo ở trạng thái Chờ duyệt");
                }

                return (true, "Đã cập nhật lý do báo cáo");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating report {ReportId} for user {UserId}", reportId, userId);
                return (false, "Có lỗi xảy ra, vui lòng thử lại");
            }
        }

        public async Task<(bool Success, string Message)> DeleteCandidateReportAsync(int userId, int reportId)
        {
            try
            {
                var candidate = await _candidateRepository.GetByUserIdAsync(userId);
                if (candidate == null)
                {
                    return (false, "Không tìm thấy ứng viên");
                }

                // Lấy thông tin report trước khi xóa để tạo notification
                var report = await _reportRepository.GetReportWithJobAsync(reportId, candidate.Id);
                if (report == null)
                {
                    return (false, "Không tìm thấy báo cáo");
                }

                if (report.Status != ReportStatus.PENDING)
                {
                    return (false, "Chỉ xóa được báo cáo ở trạng thái Chờ duyệt");
                }

                var jobId = report.JobId;
                var jobTitle = report.Job?.Title ?? "Công việc";

                var deleted = await _reportRepository.DeleteReportAsync(reportId, candidate.Id);
                if (!deleted)
                {
                    return (false, "Xóa báo cáo thất bại");
                }

                // Tạo notification sau khi xóa thành công
                await _notificationService.CreateReportDeletedAsync(candidate.Id, jobId, jobTitle);

                return (true, "Đã xóa báo cáo thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting report {ReportId} for user {UserId}", reportId, userId);
                return (false, "Có lỗi xảy ra, vui lòng thử lại");
            }
        }

        private string BuildCompanyLogo(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return "/imgs/ic_default_avatar.png";
            }

            if (imageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                return imageUrl;
            }

            try
            {
                return _storageService.BuildImageUrl(imageUrl);
            }
            catch
            {
                return "/imgs/ic_default_avatar.png";
            }
        }
    }
}
