using JobEntity = SWP391_Project.Models.Job;

namespace SWP391_Project.ViewModels.Home;

public class JobCardVM
{
    public JobEntity Job { get; set; } = null!;
    public string CompanyImageUrl { get; set; } = string.Empty;
}
