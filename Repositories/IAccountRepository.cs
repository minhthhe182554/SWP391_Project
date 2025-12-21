using SWP391_Project.Models;

namespace SWP391_Project.Repositories
{
    public interface IAccountRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(int userId);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> PhoneNumberExistsAsync(string phoneNumber);
        Task<Candidate?> GetCandidateByUserIdAsync(int userId);
        Task<Company?> GetCompanyByUserIdAsync(int userId);
        Task<Location?> GetOrCreateLocationAsync(string cityName, string wardName);
        Task<User> CreateUserAsync(User user);
        Task<Candidate> CreateCandidateAsync(Candidate candidate);
        Task<Company> CreateCompanyAsync(Company company);
        
        Task UpdateUserAsync(User user);
        Task<int> SaveChangesAsync();
    }
}
