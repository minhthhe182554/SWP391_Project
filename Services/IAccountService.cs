using SWP391_Project.Helpers;
using SWP391_Project.Models;
using SWP391_Project.Repositories;
using SWP391_Project.ViewModels;

namespace SWP391_Project.Services
{
    public interface IAccountService
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(int userId);
        Task<bool> EmailExistsAsync(string email);
        Task<(bool success, string? error)> RegisterCandidateAsync(RegisterVM model);
        Task<(bool success, string? error)> RegisterCompanyAsync(RegisterVM model, Func<string, string, Task> sendEmailAsync);
        Task<(bool success, User? user, string? error)> LoginAsync(LoginVM model);
        Task<(bool success, string? error)> VerifyAccountAsync(string token, string email);
        Task<(bool success, string? error)> ResetPasswordAsync(string email, string newPassword);
        Task<Candidate?> GetCandidateByUserIdAsync(int userId);
        Task<Company?> GetCompanyByUserIdAsync(int userId);
        Task<(bool success, string? error)> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
    }
}
