using System.Collections.Generic;
using System.Threading.Tasks;
using SWP391_Project.ViewModels.Search;

namespace SWP391_Project.Services;

public interface IDomainService
{
    Task<List<DomainOptionVM>> GetDomainsAsync(string? keyword = null);
}
