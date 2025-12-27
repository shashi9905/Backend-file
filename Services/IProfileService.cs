using WalletAPI.DTOs;

namespace WalletAPI.Services
{
    public interface IProfileService
    {
        Task<ProfileResponse?> GetProfileAsync(int userId);
        Task<bool> UpdateProfileAsync(int userId, UpdateProfileRequest request);
    }
}
