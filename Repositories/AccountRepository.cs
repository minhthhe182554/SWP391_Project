using Microsoft.EntityFrameworkCore;
using SWP391_Project.Models;

namespace SWP391_Project.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly EzJobDbContext _context;

        public AccountRepository(EzJobDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<Candidate?> GetCandidateByUserIdAsync(int userId)
        {
            return await _context.Candidates.FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Company?> GetCompanyByUserIdAsync(int userId)
        {
            return await _context.Companies.FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Location?> GetOrCreateLocationAsync(string cityName, string wardName)
        {
            var location = await _context.Locations
                .FirstOrDefaultAsync(l => l.City == cityName && l.Ward == wardName);

            if (location == null)
            {
                location = new Location
                {
                    City = cityName,
                    Ward = wardName
                };
                _context.Locations.Add(location);
                await _context.SaveChangesAsync();
            }

            return location;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<Candidate> CreateCandidateAsync(Candidate candidate)
        {
            _context.Candidates.Add(candidate);
            await _context.SaveChangesAsync();
            return candidate;
        }

        public async Task<Company> CreateCompanyAsync(Company company)
        {
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();
            return company;
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
