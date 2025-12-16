using Microsoft.EntityFrameworkCore;
using SWP391_Project.Models;

namespace SWP391_Project.Repositories
{
    public interface IDomainRepository
    {
        Task<List<Domain>> GetAllAsync();
        Task<List<Domain>> GetDomainsByIdsAsync(List<int> ids);
        Task<Domain> GetOrCreateAsync(string domainName);
    }

    public class DomainRepository : IDomainRepository
    {
        private readonly EzJobDbContext _context;
        public DomainRepository(EzJobDbContext context) => _context = context;

        public async Task<List<Domain>> GetAllAsync() => await _context.Domains.ToListAsync();

        public async Task<List<Domain>> GetDomainsByIdsAsync(List<int> ids)
        {
            return await _context.Domains.Where(d => ids.Contains(d.Id)).ToListAsync();
        }
        public async Task<Domain> GetOrCreateAsync(string domainName)
        {
            var domain = await _context.Domains.FirstOrDefaultAsync(d => d.Name == domainName);
            if (domain == null)
            {
                domain = new Domain { Name = domainName };
                _context.Domains.Add(domain);
                await _context.SaveChangesAsync();
            }
            return domain;
        }
    }
}
