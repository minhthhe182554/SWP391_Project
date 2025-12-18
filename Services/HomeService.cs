using SWP391_Project.Repositories;
using SWP391_Project.ViewModels.Home;
using SWP391_Project.ViewModels.Search;
using SWP391_Project.Services.Storage;

namespace SWP391_Project.Services;

public class HomeService : IHomeService
{
    private readonly IHomeRepository _homeRepository;
    private readonly IJobRepository _jobRepository;
    private readonly IStorageService _storageService;
    private readonly ILogger<HomeService> _logger;
    private readonly ICandidateRepository _candidateRepository;

    private const int TopDomainTake = 4;
    private const int CompanyDomainTake = 12;

    public HomeService(
        IHomeRepository homeRepository,
        IJobRepository jobRepository,
        IStorageService storageService,
        ILogger<HomeService> logger,
        ICandidateRepository candidateRepository)
    {
        _homeRepository = homeRepository;
        _jobRepository = jobRepository;
        _storageService = storageService;
        _logger = logger;
        _candidateRepository = candidateRepository;
    }

    public async Task<HomePageVM> GetHomePageAsync(HomePageFilter filter, int? candidateUserId = null)
    {
        try
        {
            var viewModel = CreateBaseViewModel(filter);
            ApplyStaticFilterOptions(viewModel);

            await LoadTopDomainsAsync(viewModel);
            await LoadJobsAsync(viewModel, filter);
            await LoadCompanyDomainOptionsAsync(viewModel);
            await LoadCompaniesAsync(viewModel, filter);
            if (candidateUserId.HasValue)
            {
                await LoadRecommendedJobsAsync(viewModel, candidateUserId.Value);
            }

            return viewModel;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading home page");
            throw;
        }
    }

    public async Task<HomePageVM> GetJobsSectionAsync(HomePageFilter filter)
    {
        try
        {
            var viewModel = CreateBaseViewModel(filter);
            ApplyStaticFilterOptions(viewModel);

            await LoadTopDomainsAsync(viewModel);
            await LoadJobsAsync(viewModel, filter);

            return viewModel;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading jobs section");
            throw;
        }
    }

    public async Task<HomePageVM> GetCompaniesSectionAsync(HomePageFilter filter)
    {
        try
        {
            var viewModel = CreateBaseViewModel(filter);

            await LoadCompanyDomainOptionsAsync(viewModel);
            await LoadCompaniesAsync(viewModel, filter);

            return viewModel;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading companies section");
            throw;
        }
    }

    public async Task<CompanySearchVM> SearchCompaniesAsync(SearchCompanyFilter filter)
    {
        try
        {
            var now = DateTime.Now;
            var companyResult = await _homeRepository.GetCompaniesAsync(new HomeCompanyQuery
            {
                Keyword = filter.Keyword,
                DomainId = filter.DomainId,
                Page = filter.Page > 0 ? filter.Page : 1,
                PageSize = filter.PageSize > 0 ? filter.PageSize : 12
            }, now);

            var domainOptions = await _homeRepository.GetCompanyDomainOptionsAsync(now, CompanyDomainTake);

            var vm = new CompanySearchVM
            {
                Filter = filter,
                DomainOptions = domainOptions
                    .Select(d => new DomainOptionVM { Id = d.Id, Name = d.Name })
                    .ToList(),
                Total = companyResult.TotalCompanies,
                TotalPages = companyResult.TotalPages,
                CurrentPage = companyResult.CurrentPage,
                PageSize = filter.PageSize > 0 ? filter.PageSize : 12
            };

            vm.CompanyCards = companyResult.Companies.Select(c =>
            {
                var activeJobs = c.Jobs.Where(j => j.EndDate >= now && !j.IsDelete).ToList();
                var domainName = activeJobs
                    .SelectMany(j => j.Domains)
                    .GroupBy(d => d.Name)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault();

                return new CompanyCardVM
                {
                    Company = c,
                    ActiveJobCount = companyResult.ActiveJobCountMap.GetValueOrDefault(c.Id, activeJobs.Count),
                    DomainName = domainName,
                    CompanyImageUrl = _storageService.BuildImageUrl(
                        !string.IsNullOrEmpty(c.ImageUrl) ? c.ImageUrl! : "default_yvl9oh")
                };
            }).ToList();

            return vm;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching companies");
            throw;
        }
    }

    private static HomePageVM CreateBaseViewModel(HomePageFilter filter)
    {
        return new HomePageVM
        {
            CurrentPage = filter.Page > 0 ? filter.Page : 1,
            PageSize = filter.PageSize,
            SelectedLocation = filter.Location,
            SelectedSalaryRange = filter.SalaryRange,
            SelectedExperience = filter.Experience,
            SelectedDomainId = filter.DomainId,
            SortOption = filter.Sort,
            SelectedCompanyDomainId = filter.CompanyDomainId,
            CompanyCurrentPage = filter.CompanyPage > 0 ? filter.CompanyPage : 1,
            CompanyPageSize = filter.CompanyPageSize
        };
    }

    private static void ApplyStaticFilterOptions(HomePageVM viewModel)
    {
        viewModel.LocationOptions = new List<string> { "Tất cả", "Hà Nội", "Hồ Chí Minh", "Đà Nẵng" };

        viewModel.SalaryRangeOptions = new List<SalaryRangeOption>
        {
            new() { Label = "dưới 10 triệu", MinSalary = 0, MaxSalary = 10_000_000 },
            new() { Label = "10-15", MinSalary = 10_000_000, MaxSalary = 15_000_000 },
            new() { Label = "15-20", MinSalary = 15_000_000, MaxSalary = 20_000_000 },
            new() { Label = "20-25", MinSalary = 20_000_000, MaxSalary = 25_000_000 },
            new() { Label = "25-30", MinSalary = 25_000_000, MaxSalary = 30_000_000 },
            new() { Label = "30-35", MinSalary = 30_000_000, MaxSalary = 35_000_000 },
            new() { Label = "35-40", MinSalary = 35_000_000, MaxSalary = 40_000_000 },
            new() { Label = "40-45", MinSalary = 40_000_000, MaxSalary = 45_000_000 },
            new() { Label = "45-50", MinSalary = 45_000_000, MaxSalary = 50_000_000 },
            new() { Label = "50+", MinSalary = 50_000_000, MaxSalary = null }
        };

        viewModel.ExperienceOptions = new List<ExperienceOption>
        {
            new() { Label = "dưới 1 năm", MinYears = 0, MaxYears = 0 },
            new() { Label = "1 năm", MinYears = 1, MaxYears = 1 },
            new() { Label = "2 năm", MinYears = 2, MaxYears = 2 },
            new() { Label = "3 năm", MinYears = 3, MaxYears = 3 },
            new() { Label = "4 năm", MinYears = 4, MaxYears = 4 },
            new() { Label = "5 năm đổ lên", MinYears = 5, MaxYears = null }
        };

        viewModel.SortOptions = new List<SortOption>
        {
            new() { Value = "newest", Label = "Mới nhất" },
            new() { Value = "salary_desc", Label = "Lương cao đến thấp" },
            new() { Value = "salary_asc", Label = "Lương thấp đến cao" }
        };
    }

    private async Task LoadTopDomainsAsync(HomePageVM viewModel)
    {
        var now = DateTime.Now;
        viewModel.TopDomains = await _homeRepository.GetTopDomainsAsync(now, TopDomainTake);
    }

    private async Task LoadJobsAsync(HomePageVM viewModel, HomePageFilter filter)
    {
        var salaryOption = viewModel.SalaryRangeOptions
            .FirstOrDefault(s => string.Equals(s.Label, filter.SalaryRange, StringComparison.OrdinalIgnoreCase));

        var experienceOption = viewModel.ExperienceOptions
            .FirstOrDefault(e => string.Equals(e.Label, filter.Experience, StringComparison.OrdinalIgnoreCase));

        var jobQuery = BuildJobQuery(filter, salaryOption, experienceOption);
        var now = DateTime.Now;
        var jobResult = await _homeRepository.GetJobsAsync(jobQuery, now);

        viewModel.TotalJobs = jobResult.Total;
        viewModel.MarketActiveJobs = jobResult.Total;
        viewModel.MarketNewJobsToday = jobResult.NewToday;
        viewModel.TotalPages = jobResult.TotalPages;
        viewModel.CurrentPage = jobResult.CurrentPage;

        viewModel.Jobs = jobResult.Jobs;
        viewModel.JobCards = jobResult.Jobs.Select(j => new JobCardVM
        {
            Job = j,
            CompanyImageUrl = _storageService.BuildImageUrl(
                !string.IsNullOrEmpty(j.Company?.ImageUrl) ? j.Company!.ImageUrl : "default_yvl9oh")
        }).ToList();
    }

    private async Task LoadCompanyDomainOptionsAsync(HomePageVM viewModel)
    {
        var now = DateTime.Now;
        viewModel.CompanyDomainOptions = await _homeRepository.GetCompanyDomainOptionsAsync(now, CompanyDomainTake);
    }

    private async Task LoadCompaniesAsync(HomePageVM viewModel, HomePageFilter filter)
    {
        var now = DateTime.Now;
        var companyResult = await _homeRepository.GetCompaniesAsync(new HomeCompanyQuery
        {
            Keyword = null,
            DomainId = filter.CompanyDomainId,
            Page = filter.CompanyPage > 0 ? filter.CompanyPage : 1,
            PageSize = filter.CompanyPageSize
        }, now);

        viewModel.CompanyTotalCompanies = companyResult.TotalCompanies;
        viewModel.CompanyTotalPages = companyResult.TotalPages;
        viewModel.CompanyCurrentPage = companyResult.CurrentPage;

        viewModel.CompanyCards = companyResult.Companies.Select(c =>
        {
            var activeJobs = c.Jobs.Where(j => j.EndDate >= now && !j.IsDelete).ToList();
            var domainName = activeJobs
                .SelectMany(j => j.Domains)
                .GroupBy(d => d.Name)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            return new CompanyCardVM
            {
                Company = c,
                ActiveJobCount = companyResult.ActiveJobCountMap.GetValueOrDefault(c.Id, activeJobs.Count),
                DomainName = domainName,
                CompanyImageUrl = _storageService.BuildImageUrl(
                    !string.IsNullOrEmpty(c.ImageUrl) ? c.ImageUrl! : "default_yvl9oh")
            };
        }).ToList();
    }

    private async Task LoadRecommendedJobsAsync(HomePageVM viewModel, int candidateUserId)
    {
        var candidate = await _candidateRepository.GetProfileByUserIdAsync(candidateUserId);
        if (candidate?.Skills == null || candidate.Skills.Count == 0)
        {
            return;
        }

        var skillIds = candidate.Skills.Select(s => s.Id).ToList();
        if (skillIds.Count == 0) return;

        var now = DateTime.Now;
        var jobs = await _jobRepository.GetRecommendedBySkillsAsync(skillIds, 3, 5, now);
        if (jobs == null || jobs.Count == 0) return;

        viewModel.RecommendedJobCards = jobs.Select(j => new JobCardVM
        {
            Job = j,
            CompanyImageUrl = _storageService.BuildImageUrl(
                !string.IsNullOrEmpty(j.Company?.ImageUrl) ? j.Company!.ImageUrl : "default_yvl9oh")
        }).ToList();
    }

    private static HomeJobQuery BuildJobQuery(HomePageFilter filter, SalaryRangeOption? salaryOption, ExperienceOption? experienceOption)
    {
        var query = new HomeJobQuery
        {
            Location = filter.Location,
            DomainId = filter.DomainId,
            Sort = filter.Sort,
            Page = filter.Page > 0 ? filter.Page : 1,
            PageSize = filter.PageSize,
            SalaryBelowTen = salaryOption != null &&
                             string.Equals(salaryOption.Label, "dưới 10 triệu", StringComparison.OrdinalIgnoreCase),
            SalaryAboveFifty = salaryOption != null &&
                               string.Equals(salaryOption.Label, "50+", StringComparison.OrdinalIgnoreCase)
        };

        if (salaryOption != null && !query.SalaryBelowTen && !query.SalaryAboveFifty)
        {
            query.MinSalary = salaryOption.MinSalary;
            query.MaxSalary = salaryOption.MaxSalary;
        }

        if (experienceOption != null)
        {
            if (string.Equals(experienceOption.Label, "dưới 1 năm", StringComparison.OrdinalIgnoreCase))
            {
                query.ExperienceBelowOne = true;
            }
            else if (string.Equals(experienceOption.Label, "5 năm đổ lên", StringComparison.OrdinalIgnoreCase))
            {
                query.ExperienceAtLeastFive = true;
            }
            else if (experienceOption.MinYears.HasValue && experienceOption.MaxYears.HasValue &&
                     experienceOption.MinYears.Value == experienceOption.MaxYears.Value)
            {
                query.ExactExperience = experienceOption.MinYears.Value;
            }
        }

        return query;
    }
}
