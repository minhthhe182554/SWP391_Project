using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP391_Project.Models;
using SWP391_Project.ViewModels.Home;
using SWP391_Project.Services.Storage;
namespace SWP391_Project.Controllers;

public class HomeController : Controller
{
    private readonly EzJobDbContext _context;
    private readonly IStorageService _storageService;
    private const int PageSize = 9;
    private const int CompanyPageSize = 12;

    public HomeController(EzJobDbContext context, IStorageService storageService)
    {
        _context = context;
        _storageService = storageService;
    }

    public IActionResult Index(int page = 1, string? location = null, string? salaryRange = null, string? experience = null, int? domainId = null, string? sort = null, int? companyDomainId = null, int companyPage = 1)
    {
        // Check if user is logged in
        var email = HttpContext.Session.GetString("Email");
        var role = HttpContext.Session.GetString("Role");
        
        if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(role))
        {
            // Redirect to role-specific home page
            if (role == "ADMIN")
            {
                return RedirectToAction("Index", "Admin");
            }
            else if (role == "COMPANY")
            {
                return RedirectToAction("Index", "Company");
            }
        }
        
        // Candidate + Guest user - show public job browsing page with pagination and filters
        var viewModel = new HomePageVM
        {
            CurrentPage = page > 0 ? page : 1,
            PageSize = PageSize,
            SelectedLocation = location,
            SelectedSalaryRange = salaryRange,
            SelectedExperience = experience,
            SelectedDomainId = domainId,
            SortOption = sort,
            SelectedCompanyDomainId = companyDomainId,
            CompanyCurrentPage = companyPage > 0 ? companyPage : 1,
            CompanyPageSize = CompanyPageSize
        };

        // Load filter options first
        LoadFilterOptions(viewModel);
        LoadTopCompanies(viewModel);

        LoadJobs(viewModel, location, salaryRange, experience, domainId, sort);
        
        return View(viewModel);
    }

    [HttpGet]
    public IActionResult JobsSection(int page = 1, string? location = null, string? salaryRange = null, string? experience = null, int? domainId = null, string? sort = null)
    {
        var viewModel = new HomePageVM
        {
            CurrentPage = page > 0 ? page : 1,
            PageSize = PageSize,
            SelectedLocation = location,
            SelectedSalaryRange = salaryRange,
            SelectedExperience = experience,
            SelectedDomainId = domainId,
            SortOption = sort
        };

        LoadFilterOptions(viewModel);
        LoadJobs(viewModel, location, salaryRange, experience, domainId, sort);

        return PartialView("_JobsSection", viewModel);
    }

    [HttpGet]
    public IActionResult CompaniesSection(int? companyDomainId = null, int companyPage = 1)
    {
        var viewModel = new HomePageVM
        {
            SelectedCompanyDomainId = companyDomainId,
            CompanyCurrentPage = companyPage > 0 ? companyPage : 1,
            CompanyPageSize = CompanyPageSize
        };

        LoadTopCompanies(viewModel);

        return PartialView("_CompaniesSection", viewModel);
    }

    private void LoadJobs(HomePageVM viewModel, string? location, string? salaryRange, string? experience, int? domainId, string? sort)
    {
        var now = DateTime.Now;

        var query = _context.Jobs
            .Include(j => j.Company)
                .ThenInclude(c => c.User)
            .Include(j => j.Location)
            .Include(j => j.RequiredSkills)
            .Include(j => j.Domains)
            .Where(j => j.EndDate >= now && !j.IsDelete && j.Company != null && j.Company.User.Active);

        // Apply filters
        if (!string.IsNullOrEmpty(location) && location != "Ngẫu nhiên")
        {
            query = query.Where(j => j.Location.City == location);
        }

        if (!string.IsNullOrEmpty(salaryRange))
        {
            var salaryOption = viewModel.SalaryRangeOptions.FirstOrDefault(s => s.Label == salaryRange);
            if (salaryOption != null)
            {
                if (salaryOption.Label == "dưới 10 triệu")
                {
                    query = query.Where(j =>
                        (j.HigherSalaryRange.HasValue && j.HigherSalaryRange < 10000000) ||
                        (j.LowerSalaryRange.HasValue && j.LowerSalaryRange < 10000000));
                }
                else if (salaryOption.Label == "50+")
                {
                    query = query.Where(j =>
                        (j.LowerSalaryRange.HasValue && j.LowerSalaryRange >= 50000000) ||
                        (j.HigherSalaryRange.HasValue && j.HigherSalaryRange >= 50000000));
                }
                else if (salaryOption.MinSalary.HasValue && salaryOption.MaxSalary.HasValue)
                {
                    // Range: jobs where salary range overlaps with filter range
                    query = query.Where(j =>
                        (j.LowerSalaryRange.HasValue && j.LowerSalaryRange <= salaryOption.MaxSalary && j.LowerSalaryRange >= salaryOption.MinSalary) ||
                        (j.HigherSalaryRange.HasValue && j.HigherSalaryRange >= salaryOption.MinSalary && j.HigherSalaryRange <= salaryOption.MaxSalary) ||
                        (j.LowerSalaryRange.HasValue && j.HigherSalaryRange.HasValue &&
                         j.LowerSalaryRange <= salaryOption.MaxSalary && j.HigherSalaryRange >= salaryOption.MinSalary));
                }
            }
        }

        if (!string.IsNullOrEmpty(experience))
        {
            if (experience == "dưới 1 năm")
            {
                query = query.Where(j => j.YearsOfExperience < 1);
            }
            else if (experience == "5 năm đổ lên")
            {
                query = query.Where(j => j.YearsOfExperience >= 5);
            }
            else if (int.TryParse(experience.Replace("năm", "").Trim(), out var years))
            {
                query = query.Where(j => j.YearsOfExperience == years);
            }
        }

        if (domainId.HasValue)
        {
            query = query.Where(j => j.Domains.Any(d => d.Id == domainId.Value));
        }

        // Order / Sort
        query = sort switch
        {
            "salary_desc" => query.OrderByDescending(j => j.HigherSalaryRange ?? j.LowerSalaryRange ?? 0),
            "salary_asc" => query.OrderBy(j => j.HigherSalaryRange ?? j.LowerSalaryRange ?? decimal.MaxValue),
            _ => query.OrderByDescending(j => j.StartDate)
        };

        // Get total count + market stats
        viewModel.TotalJobs = query.Count();
        var today = DateTime.Today;
        viewModel.MarketActiveJobs = viewModel.TotalJobs;
        viewModel.MarketNewJobsToday = query.Count(j => j.StartDate.Date == today);
        viewModel.TotalPages = (int)Math.Ceiling(viewModel.TotalJobs / (double)viewModel.PageSize);
        if (viewModel.TotalPages < 1) viewModel.TotalPages = 1;
        if (viewModel.CurrentPage > viewModel.TotalPages) viewModel.CurrentPage = viewModel.TotalPages;
        if (viewModel.CurrentPage < 1) viewModel.CurrentPage = 1;

        // Apply pagination
        var jobs = query
            .Skip((viewModel.CurrentPage - 1) * viewModel.PageSize)
            .Take(viewModel.PageSize)
            .ToList();

        viewModel.Jobs = jobs;
        viewModel.JobCards = jobs.Select(j => new JobCardVM
        {
            Job = j,
            CompanyImageUrl = _storageService.BuildImageUrl(
                !string.IsNullOrEmpty(j.Company?.ImageUrl) ? j.Company!.ImageUrl : "default_yvl9oh")
        }).ToList();
    }
    
    private void LoadFilterOptions(HomePageVM viewModel)
    {
        // Location options
        viewModel.LocationOptions = new List<string> { "Ngẫu nhiên", "Hà Nội", "Hồ Chí Minh", "Đà Nẵng" };
        
        // Salary range options
        viewModel.SalaryRangeOptions = new List<SalaryRangeOption>
        {
            new() { Label = "dưới 10 triệu", MinSalary = 0, MaxSalary = 10000000 },
            new() { Label = "10-15", MinSalary = 10000000, MaxSalary = 15000000 },
            new() { Label = "15-20", MinSalary = 15000000, MaxSalary = 20000000 },
            new() { Label = "20-25", MinSalary = 20000000, MaxSalary = 25000000 },
            new() { Label = "25-30", MinSalary = 25000000, MaxSalary = 30000000 },
            new() { Label = "30-35", MinSalary = 30000000, MaxSalary = 35000000 },
            new() { Label = "35-40", MinSalary = 35000000, MaxSalary = 40000000 },
            new() { Label = "40-45", MinSalary = 40000000, MaxSalary = 45000000 },
            new() { Label = "45-50", MinSalary = 45000000, MaxSalary = 50000000 },
            new() { Label = "50+", MinSalary = 50000000, MaxSalary = null }
        };
        
        // Experience options
        viewModel.ExperienceOptions = new List<ExperienceOption>
        {
            new() { Label = "dưới 1 năm", MinYears = 0, MaxYears = 0 },
            new() { Label = "1 năm", MinYears = 1, MaxYears = 1 },
            new() { Label = "2 năm", MinYears = 2, MaxYears = 2 },
            new() { Label = "3 năm", MinYears = 3, MaxYears = 3 },
            new() { Label = "4 năm", MinYears = 4, MaxYears = 4 },
            new() { Label = "5 năm đổ lên", MinYears = 5, MaxYears = null }
        };
        
        // Top 4 domains by job count
        viewModel.TopDomains = _context.Domains
            .Include(d => d.Jobs)
            .Where(d => d.Jobs.Any(j => j.EndDate >= DateTime.Now && !j.IsDelete))
            .Select(d => new
            {
                Domain = d,
                JobCount = d.Jobs.Count(j => j.EndDate >= DateTime.Now && !j.IsDelete)
            })
            .OrderByDescending(x => x.JobCount)
            .Take(4)
            .Select(x => x.Domain)
            .ToList();

        // Sort options
        viewModel.SortOptions = new List<SortOption>
        {
            new() { Value = "newest", Label = "Mới nhất" },
            new() { Value = "salary_desc", Label = "Lương cao đến thấp" },
            new() { Value = "salary_asc", Label = "Lương thấp đến cao" }
        };
    }

    private void LoadTopCompanies(HomePageVM viewModel)
    {
        var now = DateTime.Now;

        // Domain options for company section (top by active job count)
        viewModel.CompanyDomainOptions = _context.Domains
            .Include(d => d.Jobs)
            .Where(d => d.Jobs.Any(j => j.EndDate >= now && !j.IsDelete && j.Company != null && j.Company.User.Active))
            .Select(d => new
            {
                Domain = d,
                ActiveJobCount = d.Jobs.Count(j => j.EndDate >= now && !j.IsDelete && j.Company != null && j.Company.User.Active)
            })
            .OrderByDescending(x => x.ActiveJobCount)
            .Take(12)
            .Select(x => x.Domain)
            .ToList();

        // Companies that are actively recruiting (optionally filter by selected company domain)
        var companyBaseQuery = _context.Companies
            .Where(c => c.User.Active && c.Jobs.Any(j => j.EndDate >= now && !j.IsDelete && j.Company != null && j.Company.User.Active));

        if (viewModel.SelectedCompanyDomainId.HasValue)
        {
            var selectedId = viewModel.SelectedCompanyDomainId.Value;
            companyBaseQuery = companyBaseQuery.Where(c =>
                c.Jobs.Any(j => j.EndDate >= now && !j.IsDelete && j.Company != null && j.Company.User.Active && j.Domains.Any(d => d.Id == selectedId)));
        }

        // Company pagination
        viewModel.CompanyTotalCompanies = companyBaseQuery.Count();
        viewModel.CompanyTotalPages = (int)Math.Ceiling(viewModel.CompanyTotalCompanies / (double)viewModel.CompanyPageSize);
        if (viewModel.CompanyTotalPages < 1) viewModel.CompanyTotalPages = 1;
        if (viewModel.CompanyCurrentPage > viewModel.CompanyTotalPages) viewModel.CompanyCurrentPage = viewModel.CompanyTotalPages;
        if (viewModel.CompanyCurrentPage < 1) viewModel.CompanyCurrentPage = 1;

        // Get ordered company ids for current page (order by active job count desc)
        var pageSlice = companyBaseQuery
            .Select(c => new
            {
                c.Id,
                ActiveJobCount = c.Jobs.Count(j => j.EndDate >= now && !j.IsDelete && j.Company != null && j.Company.User.Active)
            })
            .OrderByDescending(x => x.ActiveJobCount)
            .ThenBy(x => x.Id)
            .Skip((viewModel.CompanyCurrentPage - 1) * viewModel.CompanyPageSize)
            .Take(viewModel.CompanyPageSize)
            .ToList();

        var companyIds = pageSlice.Select(x => x.Id).ToList();
        var orderIndex = pageSlice.Select((x, idx) => new { x.Id, idx }).ToDictionary(x => x.Id, x => x.idx);
        var activeJobCountMap = pageSlice.ToDictionary(x => x.Id, x => x.ActiveJobCount);

        var companiesOnPage = _context.Companies
            .Include(c => c.Jobs)
                .ThenInclude(j => j.Domains)
            .Where(c => companyIds.Contains(c.Id))
            .AsEnumerable()
            .OrderBy(c => orderIndex.GetValueOrDefault(c.Id, int.MaxValue))
            .ToList();

        viewModel.CompanyCards = companiesOnPage.Select(c =>
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
                ActiveJobCount = activeJobCountMap.GetValueOrDefault(c.Id, activeJobs.Count),
                DomainName = domainName,
                CompanyImageUrl = _storageService.BuildImageUrl(
                    !string.IsNullOrEmpty(c.ImageUrl) ? c.ImageUrl! : "default_yvl9oh")
            };
        }).ToList();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
