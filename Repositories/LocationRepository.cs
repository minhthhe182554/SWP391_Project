using Microsoft.EntityFrameworkCore;
using SWP391_Project.Models;

namespace SWP391_Project.Repositories
{
    public interface ILocationRepository
    {
        Task<List<Location>> GetAllAsync();
        Task<Location> GetOrCreateLocationAsync(string city, string? ward = null);
    }

    public class LocationRepository : ILocationRepository
    {
        private readonly EzJobDbContext _context;
        public LocationRepository(EzJobDbContext context) => _context = context;

        public async Task<List<Location>> GetAllAsync() => await _context.Locations.ToListAsync();

        public async Task<Location> GetOrCreateLocationAsync(string city, string? ward = null)
        {
            var location = await _context.Locations
        .FirstOrDefaultAsync(l => l.City == city && l.Ward == ward);

            if (location == null)
            {
                location = new Location
                {
                    City = city,
                    Ward = ward ?? string.Empty 
                };
                _context.Locations.Add(location);
                await _context.SaveChangesAsync();
            }
            return location;
        }
    }

}
