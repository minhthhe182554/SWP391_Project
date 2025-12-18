using SWP391_Project.ViewModels.Home;
using SWP391_Project.ViewModels.Search;

namespace SWP391_Project.Services;

public interface IHomeService
{
    Task<HomePageVM> GetHomePageAsync(HomePageFilter filter, int? candidateUserId = null);
    Task<HomePageVM> GetJobsSectionAsync(HomePageFilter filter);
    Task<HomePageVM> GetCompaniesSectionAsync(HomePageFilter filter);
    Task<CompanySearchVM> SearchCompaniesAsync(SearchCompanyFilter filter);
}
