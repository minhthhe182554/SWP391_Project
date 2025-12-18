using SWP391_Project.Repositories;
using SWP391_Project.ViewModels.Search;

namespace SWP391_Project.Services;

public class DomainService : IDomainService
{
    private readonly IDomainRepository _domainRepository;
    private readonly ILogger<DomainService> _logger;

    public DomainService(IDomainRepository domainRepository, ILogger<DomainService> logger)
    {
        _domainRepository = domainRepository;
        _logger = logger;
    }

    public async Task<List<DomainOptionVM>> GetDomainsAsync(string? keyword = null)
    {
        try
        {
            var domains = await _domainRepository.GetAllAsync();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                domains = domains
                    .Where(d => d.Name.Contains(keyword.Trim(), StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return domains
                .Select(d => new DomainOptionVM { Id = d.Id, Name = d.Name })
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching domains");
            throw;
        }
    }
}
