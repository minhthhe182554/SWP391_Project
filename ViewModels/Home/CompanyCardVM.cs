namespace SWP391_Project.ViewModels.Home;

public class CompanyCardVM
{
    public SWP391_Project.Models.Company Company { get; set; } = null!;
    public string CompanyImageUrl { get; set; } = string.Empty;
    public string? DomainName { get; set; }
    public int ActiveJobCount { get; set; }
}
