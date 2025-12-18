using SWP391_Project.ViewModels.Home;

namespace SWP391_Project.ViewModels.Search;

public class CompanySearchVM
{
    public SearchCompanyFilter Filter { get; set; } = new();
    public List<CompanyCardVM> CompanyCards { get; set; } = new();
    public List<DomainOptionVM> DomainOptions { get; set; } = new();
    public int Total { get; set; }
    public int TotalPages { get; set; } = 1;
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}

public class SearchCompanyFilter
{
    public string? Keyword { get; set; }
    public int? DomainId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}
