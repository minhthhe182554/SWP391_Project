using SWP391_Project.Models;
using SWP391_Project.Repositories;
using SWP391_Project.ViewModels;

namespace SWP391_Project.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly ILogger<CompanyService> _logger;

        public CompanyService(ICompanyRepository companyRepository, ILogger<CompanyService> logger)
        {
            _companyRepository = companyRepository;
            _logger = logger;
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
    }
}
