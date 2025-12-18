namespace SWP391_Project.ViewModels;

    public class AdminDashboardVM
    {
    public List<string> Labels { get; set; } = new();
    public List<int> NewUsers { get; set; } = new();
    public List<int> ActiveJobs { get; set; } = new();
    public List<int> NewApplications { get; set; } = new();
    public List<int> ActiveCompanies { get; set; } = new();
    public List<TopJobCategoryVM> TopJobCategories { get; set; } = new();
}

public class TopJobCategoryVM
{
    public string Category { get; set; } = string.Empty;
    public int Applications { get; set; }
}
