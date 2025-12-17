namespace SWP391_Project.ViewModels.Home;

public class HomePageFilter
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 9;
    public string? Location { get; set; }
    public string? SalaryRange { get; set; }
    public string? Experience { get; set; }
    public int? DomainId { get; set; }
    public string? Sort { get; set; }
    public int? CompanyDomainId { get; set; }
    public int CompanyPage { get; set; } = 1;
    public int CompanyPageSize { get; set; } = 12;
}
