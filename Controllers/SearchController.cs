using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SWP391_Project.Models;
using SWP391_Project.Services;
using SWP391_Project.ViewModels.Home;
using SWP391_Project.ViewModels.Search;

namespace SWP391_Project.Controllers;

public class SearchController : Controller
{
    private readonly IJobService _jobService;
    private readonly IDomainService _domainService;
    private readonly ILocationService _locationService;
    private readonly IHomeService _homeService;

    public SearchController(IJobService jobService, IDomainService domainService, ILocationService locationService, IHomeService homeService)
    {
        _jobService = jobService;
        _domainService = domainService;
        _locationService = locationService;
        _homeService = homeService;
    }

    [HttpGet("/search")]
    public IActionResult Index()
    {
        return RedirectToAction(nameof(Jobs));
    }

    [HttpGet("/search/jobs")]
    public async Task<IActionResult> Jobs(
        string? keyword,
        string? keywordType,
        string? cityCode,
        string? wardCode,
        [FromQuery] List<int>? domainIds,
        string? experience,
        string? salary,
        JobType? jobType,
        string? sort,
        int page = 1,
        int pageSize = 12)
    {
        var filter = new SearchFilter
        {
            Keyword = keyword,
            KeywordType = string.IsNullOrWhiteSpace(keywordType) ? "job" : keywordType,
            CityCode = cityCode,
            WardCode = wardCode,
            DomainIds = domainIds ?? new List<int>(),
            ExperienceBucket = experience,
            SalaryBucket = salary,
            JobType = jobType,
            Sort = string.IsNullOrWhiteSpace(sort) ? "date_desc" : sort,
            Page = page > 0 ? page : 1,
            PageSize = pageSize > 0 ? pageSize : 12
        };

        var cities = await _locationService.GetCitiesAsync();
        if (!string.IsNullOrWhiteSpace(cityCode))
        {
            var city = cities.FirstOrDefault(c => c.Code == cityCode);
            filter.CityName = city?.Name;
        }

        var wards = new List<SWP391_Project.Dtos.WardDto>();
        if (!string.IsNullOrWhiteSpace(cityCode))
        {
            wards = await _locationService.GetWardsByCityCodeAsync(cityCode);
            if (!string.IsNullOrWhiteSpace(wardCode))
            {
                var ward = wards.FirstOrDefault(w => w.Code == wardCode);
                filter.WardName = ward?.Name;
            }
        }

        var searchVm = await _jobService.SearchJobsAsync(filter);

        var domains = await _domainService.GetDomainsAsync();
        searchVm.Domains = domains;
        searchVm.Cities = cities;
        searchVm.Wards = wards;
        searchVm.ExperienceBuckets = BuildExperienceBuckets();
        searchVm.SalaryBuckets = BuildSalaryBuckets();
        searchVm.JobTypes = new List<JobType> { JobType.FULLTIME, JobType.PARTTIME, JobType.HYBRID };
        searchVm.SortOptions = new List<SortOption>
        {
            new() { Value = "date_desc", Label = "Ngày đăng mới nhất" },
            new() { Value = "salary_desc", Label = "Lương cao đến thấp" },
            new() { Value = "salary_asc", Label = "Lương thấp đến cao" }
        };

        return View("Index", searchVm);
    }

    [HttpGet("/search/companies")]
    public async Task<IActionResult> Companies(string? keyword, int? domainId, int page = 1, int pageSize = 12)
    {
        var filter = new SearchCompanyFilter
        {
            Keyword = keyword,
            DomainId = domainId,
            Page = page > 0 ? page : 1,
            PageSize = pageSize > 0 ? pageSize : 12
        };

        var vm = await _homeService.SearchCompaniesAsync(filter);
        return View("Companies", vm);
    }

    private static List<string> BuildExperienceBuckets()
    {
        return new List<string>
        {
            "0-1","1-2","2-3","3-4","4-5","5-6","6-7","7-8","8-9","9+"
        };
    }

    private static List<string> BuildSalaryBuckets()
    {
        return new List<string> { "all", "<1", "1-5", "5-10", "10-20", "20-50", "50+", "thoa-thuan" };
    }
}
