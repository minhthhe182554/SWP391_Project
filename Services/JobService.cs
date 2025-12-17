using SWP391_Project.Repositories;
using SWP391_Project.Services.Storage;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SWP391_Project.Models;
using SWP391_Project.ViewModels.Company;
using System.Text.Json;
using SWP391_Project.ViewModels.Job;
using SWP391_Project.ViewModels.Jobs;
using SWP391_Project.ViewModels.Search;
using SWP391_Project.ViewModels.Home;

namespace SWP391_Project.Services
{
    public class JobService : IJobService
    {
        private readonly IJobRepository _jobRepository;
        private readonly IStorageService _storageService;
        private readonly ILogger<JobService> _logger;
        private readonly ISavedJobRepository _savedJobRepository;
        private readonly ICandidateRepository _candidateRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly ISkillRepository _skillRepository;
        private readonly IDomainRepository _domainRepository;
        public JobService(ISavedJobRepository savedJobRepository,
        ICandidateRepository candidateRepository, IJobRepository jobRepository, IStorageService storageService, ILogger<JobService> logger, IApplicationRepository applicationRepository, ICompanyRepository companyRepository,
        ILocationRepository locationRepository,
        ISkillRepository skillRepository,
        IDomainRepository domainRepository)
        {
            _jobRepository = jobRepository;
            _storageService = storageService;
            _logger = logger;
            _savedJobRepository = savedJobRepository;
            _candidateRepository = candidateRepository;
            _applicationRepository = applicationRepository;
            _companyRepository = companyRepository;
            _locationRepository = locationRepository;
            _skillRepository = skillRepository;
            _domainRepository = domainRepository;
        }

        public async Task<JobDetailVM?> GetJobDetailAsync(int jobId, int? userId = null)
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
                JobType = FormatJobType(job.Type),
                IsSaved = false,
                HasApplied = false
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

            if (userId.HasValue)
            {
                var candidate = await _candidateRepository.GetByUserIdAsync(userId.Value);
                if (candidate != null)
                {
                    vm.IsSaved = await _savedJobRepository.IsSavedAsync(candidate.Id, jobId);
                    vm.HasApplied = await _applicationRepository.HasAppliedAsync(candidate.Id, jobId);
                }
            }

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
        public async Task<PostJobVM> GetPostJobModelAsync()
        {
            return new PostJobVM
            {
                Locations = await _locationRepository.GetAllAsync(),
                Domains = await _domainRepository.GetAllAsync(),
                Skills = await _skillRepository.GetAllAsync(),
                EndDate = DateTime.Now.AddMonths(1)
            };
        }

        public class TagItem
        {
            public string value { get; set; }
        }

        public async Task AddJobAsync(int userId, PostJobVM model)
        {
            var company = await _companyRepository.GetByUserIdAsync(userId);
            if (company == null) throw new Exception("Không tìm thấy thông tin công ty.");

            var location = await _locationRepository.GetOrCreateLocationAsync(model.CityName, model.WardName);

            var job = new Job
            {
                CompanyId = company.Id,
                Title = model.Title,
                LocationId = location.Id,
                Address = model.Address,
                LowerSalaryRange = model.LowerSalary,
                HigherSalaryRange = model.HigherSalary,
                YearsOfExperience = model.YearsOfExperience,
                VacancyCount = model.VacancyCount,
                Description = model.Description,
                StartDate = DateTime.Now,
                EndDate = model.EndDate,
                Type = model.JobType,
                IsDelete = false
            };

            if (!string.IsNullOrEmpty(model.SelectedSkills))
            {
                try
                {
                    var tags = JsonSerializer.Deserialize<List<TagItem>>(model.SelectedSkills);
                    if (tags != null)
                    {
                        foreach (var tag in tags)
                        {
                            var skill = await _skillRepository.GetOrCreateAsync(tag.value);
                            job.RequiredSkills.Add(skill);
                        }
                    }
                }
                catch
                {
                    throw new Exception("Lỗi định dạng kỹ năng.");
                }
            }

            if (!string.IsNullOrEmpty(model.SelectedDomains))
            {
                try
                {
                    var tags = JsonSerializer.Deserialize<List<TagItem>>(model.SelectedDomains);
                    if (tags != null)
                    {
                        foreach (var tag in tags)
                        {
                            var domain = await _domainRepository.GetOrCreateAsync(tag.value);
                            job.Domains.Add(domain);
                        }
                    }
                }
                catch
                {
                    throw new Exception("Lỗi định dạng lĩnh vực.");
                }
            }

            await _jobRepository.AddAsync(job);
        }

        public async Task<List<ManageJobsVM>> GetCompanyJobsAsync(int userId)
        {
            var company = await _companyRepository.GetByUserIdAsync(userId);
            if (company == null) return new List<ManageJobsVM>();

            var jobs = await _jobRepository.GetJobsByCompanyIdAsync(company.Id);

            return jobs.Select(j => new ManageJobsVM
            {
                Id = j.Id,
                Title = j.Title,
                CityName = j.Location?.City ?? "N/A",
                StartDate = j.StartDate,
                EndDate = j.EndDate,
                JobType = j.Type,
                ApplicationCount = j.Applications.Count
            }).ToList();
        }

        public async Task RepostJobAsync(int userId, int jobId)
        {
            var company = await _companyRepository.GetByUserIdAsync(userId);
            if (company == null) throw new Exception("Company not found");

            var oldJob = await _jobRepository.GetJobWithDetailsAsync(jobId);

            if (oldJob == null) throw new Exception("Job not found");
            if (oldJob.CompanyId != company.Id) throw new Exception("Unauthorized access");

            var newJob = new Job
            {
                CompanyId = company.Id,
                Title = oldJob.Title + " (Repost)",
                Description = oldJob.Description,
                Type = oldJob.Type,
                YearsOfExperience = oldJob.YearsOfExperience,
                VacancyCount = oldJob.VacancyCount,
                LowerSalaryRange = oldJob.LowerSalaryRange,
                HigherSalaryRange = oldJob.HigherSalaryRange,
                Address = oldJob.Address,
                LocationId = oldJob.LocationId,

                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(30),

                IsDelete = false,

                RequiredSkills = oldJob.RequiredSkills.ToList(),
                Domains = oldJob.Domains.ToList(),

                Applications = new List<Application>(),
                Reports = new List<Report>()
            };

            await _jobRepository.AddAsync(newJob);
        }

        public async Task StopRecruitmentAsync(int userId, int jobId)
        {
            var job = await _jobRepository.GetByIdAsync(jobId);
            if (job == null) throw new Exception("Job not found");

            var company = await _companyRepository.GetByUserIdAsync(userId);
            if (company == null || job.CompanyId != company.Id) throw new Exception("Unauthorized");

            job.EndDate = DateTime.Now.AddMinutes(-1);

            // Nếu Repo chưa có Update, bạn thêm vào Repo: _context.Jobs.Update(job); await _context.SaveChangesAsync();
            await _jobRepository.UpdateAsync(job);
        }

        public async Task<bool> CanEditJobAsync(int jobId)
        {
            var job = await _jobRepository.GetJobWithDetailsAsync(jobId);
            if (job == null) return false;

            // Nếu số lượng Application > 0 thì KHÔNG ĐƯỢC SỬA
            return job.Applications.Count == 0;
        }
            public async Task<SearchPageVM> SearchJobsAsync(SearchFilter filter)
            {
                try
                {
                    var page = filter.Page > 0 ? filter.Page : 1;
                    var pageSize = filter.PageSize > 0 ? filter.PageSize : 10;

                    var (minExp, maxExp) = MapExperienceBucket(filter.ExperienceBucket);
                    var (minSalary, maxSalary) = MapSalaryBucket(filter.SalaryBucket);

                    var query = new JobSearchQuery
                    {
                        Keyword = filter.Keyword,
                        KeywordType = string.IsNullOrWhiteSpace(filter.KeywordType) ? "job" : filter.KeywordType,
                        City = filter.CityName,
                        Ward = filter.WardName,
                        DomainIds = filter.DomainIds ?? new System.Collections.Generic.List<int>(),
                        MinExperience = minExp,
                        MaxExperience = maxExp,
                        MinSalary = minSalary,
                        MaxSalary = maxSalary,
                        JobType = filter.JobType,
                        Sort = string.IsNullOrWhiteSpace(filter.Sort) ? "date_desc" : filter.Sort,
                        Page = page,
                        PageSize = pageSize
                    };

                    var now = DateTime.Now;
                    var (jobs, total) = await _jobRepository.SearchAsync(query, now);
                    var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
                    var currentPage = Math.Min(page, totalPages);

                    var cards = jobs.Select(j => new JobCardVM
                    {
                        Job = j,
                        CompanyImageUrl = _storageService.BuildImageUrl(
                            !string.IsNullOrEmpty(j.Company?.ImageUrl) ? j.Company!.ImageUrl : "default_yvl9oh")
                    }).ToList();

                    filter.Page = currentPage;
                    filter.PageSize = pageSize;

                    return new SearchPageVM
                    {
                        Filter = filter,
                        JobCards = cards,
                        TotalResults = total,
                        TotalPages = totalPages,
                        CurrentPage = currentPage,
                        PageSize = pageSize
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error searching jobs");
                    throw;
                }
            }

            private static (int? min, int? max) MapExperienceBucket(string? bucket)
            {
                return bucket switch
                {
                    "0-1" => (0, 1),
                    "1-2" => (1, 2),
                    "2-3" => (2, 3),
                    "3-4" => (3, 4),
                    "4-5" => (4, 5),
                    "5-6" => (5, 6),
                    "6-7" => (6, 7),
                    "7-8" => (7, 8),
                    "8-9" => (8, 9),
                    "9+" => (9, null),
                    _ => (null, null)
                };
            }

            private static (decimal? min, decimal? max) MapSalaryBucket(string? bucket)
            {
                return bucket switch
                {
                    "<1" => (0, 1_000_000),
                    "1-5" => (1_000_000, 5_000_000),
                    "5-10" => (5_000_000, 10_000_000),
                    "10-20" => (10_000_000, 20_000_000),
                    "20-50" => (20_000_000, 50_000_000),
                    "50+" => (50_000_000, null),
                    _ => (null, null)
                };
            }
        }
    }
