using SWP391_Project.Repositories;
using SWP391_Project.Services.Storage;
using SWP391_Project.ViewModels.Jobs;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SWP391_Project.Services
{
    public class JobService : IJobService
    {
        private readonly IJobRepository _jobRepository;
        private readonly IStorageService _storageService;
        private readonly ILogger<JobService> _logger;

        public JobService(IJobRepository jobRepository, IStorageService storageService, ILogger<JobService> logger)
        {
            _jobRepository = jobRepository;
            _storageService = storageService;
            _logger = logger;
        }

        public async Task<JobDetailVM?> GetJobDetailAsync(int jobId)
        {
            var job = await _jobRepository.GetJobWithDetailsAsync(jobId);
            if (job == null) return null;

            var salaryText = FormatSalary(job.LowerSalaryRange, job.HigherSalaryRange);
            var locationLabel = job.Location?.City ?? job.Company?.Location?.City ?? "N/A";
            var experienceLabel = job.YearsOfExperience > 0 ? $"{job.YearsOfExperience} năm kinh nghiệm" : "Không yêu cầu";
            var deadlineLabel = job.EndDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            var companyImage = BuildCompanyImage(job.Company?.ImageUrl);

            var vm = new JobDetailVM
            {
                Id = job.Id,
                CompanyId = job.CompanyId,
                Title = job.Title,
                SalaryText = salaryText,
                Location = locationLabel,
                Experience = experienceLabel,
                Deadline = deadlineLabel,
                Description = job.Description,
                Skills = job.RequiredSkills.Select(s => s.Name).ToList(),
                Domains = job.Domains.Select(d => d.Name).ToList(),
                CompanyImageUrl = companyImage,
                CompanyName = job.Company?.Name ?? "Công ty",
                CompanyAddress = job.Company?.Address ?? job.Address ?? "Địa điểm chưa rõ",
                CompanyDomains = job.Domains.Select(d => d.Name).Take(2).ToList(),
                VacancyCount = job.VacancyCount,
                JobType = FormatJobType(job.Type)
            };

            // Similar jobs
            var similarJobs = await _jobRepository.GetActiveJobsWithDetailsAsync();
            var currentDomainIds = job.Domains.Select(d => d.Id).ToHashSet();
            var currentSkillIds = job.RequiredSkills.Select(s => s.Id).ToHashSet();

            var filtered = similarJobs
                .Where(j => j.Id != job.Id && j.YearsOfExperience <= job.YearsOfExperience)
                .Select(j =>
                {
                    var domainOverlap = j.Domains.Count(d => currentDomainIds.Contains(d.Id));
                    var skillOverlap = j.RequiredSkills.Count(s => currentSkillIds.Contains(s.Id));
                    return new { Job = j, DomainOverlap = domainOverlap, SkillOverlap = skillOverlap };
                })
                .Where(x => x.DomainOverlap >= 1 && x.SkillOverlap >= 3)
                .OrderByDescending(x => x.DomainOverlap + x.SkillOverlap)
                .ThenByDescending(x => x.SkillOverlap)
                .Take(3)
                .ToList();

            vm.SimilarJobs = filtered.Select(x => new SimilarJobCardVM
            {
                Id = x.Job.Id,
                Title = x.Job.Title,
                CompanyName = x.Job.Company?.Name ?? "Công ty",
                Location = x.Job.Location?.City ?? x.Job.Company?.Location?.City ?? "N/A",
                SalaryText = FormatSalary(x.Job.LowerSalaryRange, x.Job.HigherSalaryRange),
                Experience = x.Job.YearsOfExperience > 0 ? $"{x.Job.YearsOfExperience} năm kinh nghiệm" : "Không yêu cầu",
                CompanyImageUrl = BuildCompanyImage(x.Job.Company?.ImageUrl)
            }).ToList();

            _logger.LogInformation("Similar jobs for {JobId}: {Count}", jobId, vm.SimilarJobs.Count);

            return vm;
        }

        private static string FormatSalary(decimal? lower, decimal? higher)
        {
            if (lower.HasValue && higher.HasValue)
            {
                return $"{lower.Value:N0} - {higher.Value:N0} VNĐ";
            }
            if (lower.HasValue)
            {
                return $"Từ {lower.Value:N0} VNĐ";
            }
            if (higher.HasValue)
            {
                return $"Đến {higher.Value:N0} VNĐ";
            }
            return "Thoả thuận";
        }

        private string BuildCompanyImage(string? imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
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

        private static string FormatJobType(Models.JobType type)
        {
            return type switch
            {
                Models.JobType.FULLTIME => "Toàn thời gian",
                Models.JobType.PARTTIME => "Bán thời gian",
                Models.JobType.HYBRID => "Hybrid",
                _ => type.ToString()
            };
        }
    }
}

