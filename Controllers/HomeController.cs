using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SWP391_Project.Models;
using SWP391_Project.ViewModels.Home;
using SWP391_Project.Services;
namespace SWP391_Project.Controllers;

public class HomeController : Controller
{
    private readonly IHomeService _homeService;
    private const int PageSize = 9;
    private const int CompanyPageSize = 12;

    public HomeController(IHomeService homeService)
    {
        _homeService = homeService;
    }

    public async Task<IActionResult> Index(int page = 1, string? location = null, string? salaryRange = null, string? experience = null, int? domainId = null, string? sort = null, int? companyDomainId = null, int companyPage = 1)
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
        
        int? candidateUserId = null;
        if (role == "CANDIDATE")
        {
            var userIdStr = HttpContext.Session.GetString("UserID");
            if (int.TryParse(userIdStr, out var uid)) candidateUserId = uid;
        }

        var filter = new HomePageFilter
        {
            Page = page > 0 ? page : 1,
            PageSize = PageSize,
            Location = location,
            SalaryRange = salaryRange,
            Experience = experience,
            DomainId = domainId,
            Sort = sort,
            CompanyDomainId = companyDomainId,
            CompanyPage = companyPage > 0 ? companyPage : 1,
            CompanyPageSize = CompanyPageSize
        };

        var viewModel = await _homeService.GetHomePageAsync(filter, candidateUserId);
        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> JobsSection(int page = 1, string? location = null, string? salaryRange = null, string? experience = null, int? domainId = null, string? sort = null)
    {
        var filter = new HomePageFilter
        {
            Page = page > 0 ? page : 1,
            PageSize = PageSize,
            Location = location,
            SalaryRange = salaryRange,
            Experience = experience,
            DomainId = domainId,
            Sort = sort
        };

        var viewModel = await _homeService.GetJobsSectionAsync(filter);

        return PartialView("_JobsSection", viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> CompaniesSection(int? companyDomainId = null, int companyPage = 1)
    {
        var filter = new HomePageFilter
        {
            CompanyDomainId = companyDomainId,
            CompanyPage = companyPage > 0 ? companyPage : 1,
            CompanyPageSize = CompanyPageSize
        };

        var viewModel = await _homeService.GetCompaniesSectionAsync(filter);

        return PartialView("_CompaniesSection", viewModel);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
