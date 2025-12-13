using Microsoft.EntityFrameworkCore;
using SWP391_Project.Models;

namespace SWP391_Project.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly EzJobDbContext _context;

        public CompanyRepository(EzJobDbContext context)
        {
            _context = context;
        }

        public async Task<Company?> GetByUserIdAsync(int userId)
        {
            return await _context.Companies
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<int> GetTotalJobsAsync(int companyId)
        {
            return await _context.Jobs.CountAsync(j => j.CompanyId == companyId && !j.IsDelete);
        }

        public async Task<int> GetActiveJobsAsync(int companyId)
        {
            return await _context.Jobs.CountAsync(j => 
                j.CompanyId == companyId && 
                j.EndDate >= DateTime.Now && 
                !j.IsDelete);
        }

        public async Task<int> GetTotalApplicationsAsync(int companyId)
        {
            return await _context.Applications
                .CountAsync(a => _context.Jobs.Any(j => 
                    j.CompanyId == companyId && 
                    j.Id == a.JobId));
        }

        public async Task<Company?> GetByIdAsync(int id)
        {
            return await _context.Companies
                .Include(c => c.Location)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Company?> GetDetailAsync(int id)
        {
            return await _context.Companies
                .Include(c => c.Location)
                .Include(c => c.Jobs)
                    .ThenInclude(j => j.Domains)
                .Include(c => c.Jobs)
                    .ThenInclude(j => j.RequiredSkills)
                .Include(c => c.Jobs)
                    .ThenInclude(j => j.Location)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task UpdateAsync(Company company)
        {
            _context.Companies.Update(company);
            await _context.SaveChangesAsync();
        }

        public async Task<Location> GetOrCreateLocationAsync(string city, string ward)
        {
            var location = await _context.Locations
                .FirstOrDefaultAsync(l => l.City == city && l.Ward == ward);

            if (location == null)
            {
                location = new Location { City = city, Ward = ward };
                _context.Locations.Add(location);
                await _context.SaveChangesAsync();
            }
            return location;
        }
    }
}
