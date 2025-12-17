using System.Collections.Generic;
using SWP391_Project.Dtos;
using SWP391_Project.Models;
using SWP391_Project.ViewModels.Home;

namespace SWP391_Project.ViewModels.Search;

public class SearchPageVM
{
    public SearchFilter Filter { get; set; } = new();
    public List<JobCardVM> JobCards { get; set; } = new();
    public int TotalResults { get; set; }
    public int TotalPages { get; set; } = 1;
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public List<DomainOptionVM> Domains { get; set; } = new();
    public List<CityDto> Cities { get; set; } = new();
    public List<WardDto> Wards { get; set; } = new();

    public List<string> ExperienceBuckets { get; set; } = new();
    public List<string> SalaryBuckets { get; set; } = new();
    public List<JobType> JobTypes { get; set; } = new();
    public List<SortOption> SortOptions { get; set; } = new();
}

public class SearchFilter
{
    public string? Keyword { get; set; }
    public string KeywordType { get; set; } = "job"; // job | company
    public string? CityCode { get; set; }
    public string? WardCode { get; set; }
    public string? CityName { get; set; }
    public string? WardName { get; set; }
    public List<int> DomainIds { get; set; } = new();
    public string? ExperienceBucket { get; set; }
    public string? SalaryBucket { get; set; }
    public JobType? JobType { get; set; }
    public string Sort { get; set; } = "date_desc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class DomainOptionVM
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
