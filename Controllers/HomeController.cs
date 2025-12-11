using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP391_Project.Models;
using SWP391_Project.ViewModels.Home;

namespace SWP391_Project.Controllers;

public class HomeController : Controller
{
    private readonly EzJobDbContext _context;
    private const int PageSize = 9;

    public HomeController(EzJobDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(int page = 1, string? location = null, string? salaryRange = null, string? experience = null, int? domainId = null)
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
            else if (role == "CANDIDATE")
            {
                return RedirectToAction("Index", "Candidate");
            }
        }
        
        // Guest user - show public job browsing page with pagination and filters
        var viewModel = new HomePageVM
        {
            CurrentPage = page > 0 ? page : 1,
            PageSize = PageSize,
            SelectedLocation = location,
            SelectedSalaryRange = salaryRange,
            SelectedExperience = experience,
            SelectedDomainId = domainId
        };
        
        // Build query
        var query = _context.Jobs
            .Include(j => j.Company)
            .Include(j => j.Location)
            .Include(j => j.RequiredSkills)
            .Include(j => j.Domains)
            .Where(j => j.EndDate >= DateTime.Now && !j.IsDelete);
        
        // Load filter options first
        LoadFilterOptions(viewModel);
        
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
        
        // Order by StartDate descending (newest first)
        query = query.OrderByDescending(j => j.StartDate);
        
        // Get total count
        viewModel.TotalJobs = query.Count();
        viewModel.TotalPages = (int)Math.Ceiling(viewModel.TotalJobs / (double)PageSize);
        
        // Apply pagination
        var jobs = query
            .Skip((viewModel.CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToList();
        
        viewModel.Jobs = jobs;
        
        return View(viewModel);
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
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
