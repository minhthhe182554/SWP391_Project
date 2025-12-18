using SWP391_Project.Helpers;
using SWP391_Project.Models;
using SWP391_Project.Models.Enums;
using SWP391_Project.Repositories;
using SWP391_Project.ViewModels;

namespace SWP391_Project.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<AccountService> _logger;

        public AccountService(IAccountRepository accountRepository, ILogger<AccountService> logger)
        {
            _accountRepository = accountRepository;
            _logger = logger;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            try
            {
                return await _accountRepository.GetUserByEmailAsync(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by email");
                throw;
            }
        }
        
        public async Task<User?> GetUserByIdAsync(int userId)
        {
            try
            {
                return await _accountRepository.GetUserByIdAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by id {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            try
            {
                return await _accountRepository.EmailExistsAsync(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if email exists: {Email}", email);
                throw;
            }
        }

        public async Task<(bool success, string? error)> RegisterCandidateAsync(RegisterVM model)
        {
            try
            {
                if (await _accountRepository.EmailExistsAsync(model.Email))
                {
                    
                    return (false, "Email này đã được sử dụng");
                }

                var newUser = new User
                {
                    Email = model.Email,
                    Password = HashHelper.Hash(model.Password),
                    Role = Role.CANDIDATE,
                    Active = false
                };
                await _accountRepository.CreateUserAsync(newUser);

                var candidate = new Candidate
                {
                    UserId = newUser.Id,
                    FullName = model.FullName,
                    Jobless = true,
                    RemainingReport = 2
                };
                await _accountRepository.CreateCandidateAsync(candidate);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering candidate");
                return (false, "Có lỗi xảy ra khi đăng ký");
            }
        }

        public async Task<(bool success, string? error)> RegisterCompanyAsync(RegisterVM model, Func<string, string, Task> sendEmailAsync)
        {
            try
            {
                if (await _accountRepository.EmailExistsAsync(model.Email))
                {
                    return (false, "Email này đã được sử dụng");
                }

                // Get or create location
                var location = await _accountRepository.GetOrCreateLocationAsync(model.City!, model.Ward!);
                
                if (location == null)
                {
                    return (false, "Không thể tạo vị trí cho công ty");
                }

                var newUser = new User
                {
                    Email = model.Email,
                    Password = HashHelper.Hash(model.Password),
                    Role = Role.COMPANY,
                    Active = false
                };
                await _accountRepository.CreateUserAsync(newUser);

                var company = new Company
                {
                    UserId = newUser.Id,
                    Name = model.FullName,
                    Description = model.Description!,
                    Address = model.Address!,
                    PhoneNumber = model.PhoneNumber!,
                    LocationId = location.Id
                };
                await _accountRepository.CreateCompanyAsync(company);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering company");
                return (false, "Có lỗi xảy ra khi đăng ký");
            }
        }

        public async Task<(bool success, User? user, string? error)> LoginAsync(LoginVM model)
        {
            try
            {
                var user = await _accountRepository.GetUserByEmailAsync(model.Email);

                if (user == null || !HashHelper.Verify(model.Password, user.Password))
                {
                    return (false, null, "Sai tài khoản hoặc mật khẩu");
                }

                if (!user.Active)
                {
                    return (false, null, "Tài khoản hiện đang bị khóa hoặc chưa được kích hoạt");
                }

                return (true, user, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return (false, null, "Có lỗi xảy ra khi đăng nhập");
            }
        }

        public async Task<(bool success, string? error)> VerifyAccountAsync(string token, string email)
        {
            try
            {
                var user = await _accountRepository.GetUserByEmailAsync(email);

                if (user == null)
                {
                    return (false, "Không tìm thấy tài khoản");
                }

                if (user.Active)
                {
                    return (false, "Tài khoản này đã được kích hoạt trước đó rồi");
                }

                user.Active = true;
                await _accountRepository.UpdateUserAsync(user);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying account");
                return (false, "Có lỗi xảy ra khi xác thực tài khoản");
            }
        }

        public async Task<(bool success, string? error)> ResetPasswordAsync(string email, string newPassword)
        {
            try
            {
                var user = await _accountRepository.GetUserByEmailAsync(email);

                if (user == null)
                {
                    return (false, "Không tìm thấy tài khoản");
                }

                user.Password = HashHelper.Hash(newPassword);
                await _accountRepository.UpdateUserAsync(user);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password");
                return (false, "Có lỗi xảy ra khi đặt lại mật khẩu");
            }
        }

        public async Task<Candidate?> GetCandidateByUserIdAsync(int userId)
        {
            try
            {
                return await _accountRepository.GetCandidateByUserIdAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting candidate by user id {UserId}", userId);
                throw;
            }
        }

        public async Task<Company?> GetCompanyByUserIdAsync(int userId)
        {
            try
            {
                return await _accountRepository.GetCompanyByUserIdAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting company by user id {UserId}", userId);
                throw;
            }
        }

        public async Task<(bool success, string? error)> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            try
            {
                var user = await _accountRepository.GetUserByIdAsync(userId);
                if(user == null)
                {
                    return (false, "Người dùng không tồn tại");
                }
                if(!HashHelper.Verify(oldPassword, user.Password))
                {
                    return (false, "Mật khẩu cũ không đúng");
                }

                user.Password = HashHelper.Hash(newPassword);
                await _accountRepository.UpdateUserAsync(user);

                return (true, null);
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user {UserId}", userId);
                return (false, "Có lỗi xảy ra khi đổi mật khẩu");
            }
        }
    }
}
