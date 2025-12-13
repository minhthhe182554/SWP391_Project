using SWP391_Project.Models;
using SWP391_Project.Repositories;
using SWP391_Project.ViewModels.Company;
using Microsoft.AspNetCore.Hosting;
using SWP391_Project.ViewModels;
using SWP391_Project.Services.Storage;
using System.Globalization;
using System.Linq;

namespace SWP391_Project.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly ILogger<CompanyService> _logger;
        private readonly IStorageService _storageService;

        public CompanyService(ICompanyRepository companyRepository, ILogger<CompanyService> logger, IStorageService storageService)
        {
            _companyRepository = companyRepository;
            _logger = logger;
            _storageService = storageService;
        }

        public async Task<Company?> GetCompanyByUserIdAsync(int userId)
        {
            try
            {
                return await _companyRepository.GetByUserIdAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting company by user id {UserId}", userId);
                throw;
            }
        }

        public async Task<CompanyDashboardVM> GetCompanyDashboardViewAsync(int companyId)
        {
            try
            {
                var totalJobs = await _companyRepository.GetTotalJobsAsync(companyId);
                var activeJobs = await _companyRepository.GetActiveJobsAsync(companyId);
                var totalApplications = await _companyRepository.GetTotalApplicationsAsync(companyId);

                return new CompanyDashboardVM
                {
                    TotalJobs = totalJobs,
                    ActiveJobs = activeJobs,
                    TotalApplications = totalApplications
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting company dashboard for company {CompanyId}", companyId);
                throw;
            }
        }

        public async Task<CompanyProfileVM?> GetProfileForEditAsync(int userId)
        {
            var company = await _companyRepository.GetByUserIdAsync(userId);
            if (company == null) return null;

            var fullInfo = await _companyRepository.GetByIdAsync(company.Id);

            return new CompanyProfileVM
            {
                Id = fullInfo!.Id,
                Name = fullInfo.Name,
                PhoneNumber = fullInfo.PhoneNumber,
                Website = fullInfo.Website,
                Description = fullInfo.Description,
                Address = fullInfo.Address,
                City = fullInfo.Location.City,
                Ward = fullInfo.Location.Ward,
                ExistingImageUrl = fullInfo.ImageUrl,
                Latitude = fullInfo.Latitude,
                Longitude = fullInfo.Longitude
            };
        }

        public async Task<bool> UpdateProfileAsync(int userId, CompanyProfileVM model)
        {
            try
            {
                var company = await _companyRepository.GetByUserIdAsync(userId);
                if (company == null) return false;

                var location = await _companyRepository.GetOrCreateLocationAsync(model.City, model.Ward);

                if (model.AvatarFile != null)
                {
                    string fixedPublicId = $"avatar_{userId}";

                    string publicId = await _storageService.UploadImageAsync(model.AvatarFile, "companies", fixedPublicId);

                    string secureUrl = _storageService.BuildImageUrl(publicId);

                    company.ImageUrl = secureUrl;
                }

                company.Name = model.Name;
                company.PhoneNumber = model.PhoneNumber;
                company.Website = model.Website;
                company.Description = model.Description;
                company.Address = model.Address;
                company.LocationId = location.Id;
                company.Latitude = model.Latitude;
                company.Longitude = model.Longitude;

                await _companyRepository.UpdateAsync(company);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật hồ sơ công ty user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> UpdateBasicProfileAsync(int userId, CompanyProfileVM model)
        {
            try
            {
                var company = await _companyRepository.GetByUserIdAsync(userId);
                if (company == null) return false;

                if (model.AvatarFile != null)
                {
                    string fixedPublicId = $"avatar_{userId}";
                    string publicId = await _storageService.UploadImageAsync(model.AvatarFile, "companies", fixedPublicId);
                    string secureUrl = _storageService.BuildImageUrl(publicId);
                    company.ImageUrl = secureUrl;
                }

                company.Name = model.Name;
                company.PhoneNumber = model.PhoneNumber;
                company.Website = model.Website;
                company.Description = model.Description;

                await _companyRepository.UpdateAsync(company);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật thông tin cơ bản công ty user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> UpdateAddressProfileAsync(int userId, CompanyProfileVM model)
        {
            try
            {
                var company = await _companyRepository.GetByUserIdAsync(userId);
                if (company == null) return false;

                var location = await _companyRepository.GetOrCreateLocationAsync(model.City, model.Ward);

                company.Address = model.Address;
                company.LocationId = location.Id;
                company.Latitude = model.Latitude;
                company.Longitude = model.Longitude;

                await _companyRepository.UpdateAsync(company);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật địa chỉ công ty user {UserId}", userId);
                return false;
            }
        }

        public async Task<CompanyDetailVM?> GetCompanyDetailAsync(int companyId)
        {
            var company = await _companyRepository.GetDetailAsync(companyId);
            if (company == null) return null;

            var activeJobs = company.Jobs
                .Where(j => !j.IsDelete && j.EndDate >= DateTime.Now)
                .ToList();

            var domains = activeJobs
                .SelectMany(j => j.Domains)
                .Select(d => d.Name)
                .Distinct()
                .ToList();

            var jobsVm = activeJobs.Select(j => new CompanyJobItemVM
            {
                Id = j.Id,
                Title = j.Title,
                SalaryText = FormatSalary(j.LowerSalaryRange, j.HigherSalaryRange),
                Location = j.Location?.City ?? company.Location?.City ?? "N/A",
                Experience = j.YearsOfExperience > 0 ? $"{j.YearsOfExperience} năm kinh nghiệm" : "Không yêu cầu",
                JobType = FormatJobType(j.Type),
                CompanyImageUrl = BuildImageUrl(company.ImageUrl)
            }).ToList();

            var vm = new CompanyDetailVM
            {
                Id = company.Id,
                Name = company.Name,
                Website = company.Website,
                Description = company.Description,
                Address = company.Address,
                City = company.Location?.City,
                Ward = company.Location?.Ward,
                Latitude = company.Latitude,
                Longitude = company.Longitude,
                ImageUrl = BuildImageUrl(company.ImageUrl),
                Domains = domains,
                Jobs = jobsVm
            };

            return vm;
        }

        private string BuildImageUrl(string? imageUrl)
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

        private static string FormatJobType(JobType type)
        {
            return type switch
            {
                JobType.FULLTIME => "Toàn thời gian",
                JobType.PARTTIME => "Bán thời gian",
                JobType.HYBRID => "Hybrid",
                _ => type.ToString()
            };
        }
    }
}
