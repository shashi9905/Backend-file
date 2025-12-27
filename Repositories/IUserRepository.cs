using WalletAPI.Models;

namespace WalletAPI.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int userId);
        Task<User?> GetByMobileAsync(string mobile);
        Task<User?> GetByEmailAsync(string email);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<bool> MobileExistsAsync(string mobile);
        Task<bool> EmailExistsAsync(string email);
    }
}
