using Microsoft.EntityFrameworkCore;
using SWP391_Project.Models;

namespace SWP391_Project.Repositories
{
    public interface ILocationRepository
    {
        Task<List<Location>> GetAllAsync();
    }

    public class LocationRepository : ILocationRepository
    {
        private readonly EzJobDbContext _context;
        public LocationRepository(EzJobDbContext context) => _context = context;

        public async Task<List<Location>> GetAllAsync() => await _context.Locations.ToListAsync();
    }
}
