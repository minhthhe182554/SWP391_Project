using SWP391_Project.Models;
using SWP391_Project.Repositories;
using SWP391_Project.ViewModels.Company;
using Microsoft.AspNetCore.Hosting;
using SWP391_Project.Helpers;
using SWP391_Project.ViewModels;

namespace SWP391_Project.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly ILogger<CompanyService> _logger;
        private readonly ICloudinaryHelper _cloudinaryHelper;

        public CompanyService(ICompanyRepository companyRepository, ILogger<CompanyService> logger, ICloudinaryHelper cloudinaryHelper)
        {
            _companyRepository = companyRepository;
            _logger = logger;
            _cloudinaryHelper = cloudinaryHelper;
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
                ExistingImageUrl = fullInfo.ImageUrl 
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

                    string publicId = await _cloudinaryHelper.UploadImageAsync(model.AvatarFile, "companies", fixedPublicId);

                    string secureUrl = _cloudinaryHelper.BuildImageUrl(publicId);

                    company.ImageUrl = secureUrl;
                }

                company.Name = model.Name;
                company.PhoneNumber = model.PhoneNumber;
                company.Website = model.Website;
                company.Description = model.Description;
                company.Address = model.Address;
                company.LocationId = location.Id;

                await _companyRepository.UpdateAsync(company);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật hồ sơ công ty user {UserId}", userId);
                return false;
            }
        }
    }
}
