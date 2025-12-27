using WalletAPI.DTOs;
using WalletAPI.Repositories;

namespace WalletAPI.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUserRepository _userRepository;
        private readonly IWalletService _walletService;

        public ProfileService(IUserRepository userRepository, IWalletService walletService)
        {
            _userRepository = userRepository;
            _walletService = walletService;
        }

        public async Task<ProfileResponse?> GetProfileAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            var wallet = await _walletService.GetWalletAsync(userId);

            return new ProfileResponse
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Mobile = user.Mobile,
                Email = user.Email,
                IsActive = user.IsActive,
                CreatedOn = user.CreatedOn,
                Wallet = wallet
            };
        }

        public async Task<bool> UpdateProfileAsync(int userId, UpdateProfileRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(request.FullName))
            {
                user.FullName = request.FullName;
            }

            if (!string.IsNullOrEmpty(request.Email))
            {
                // Check if email is already used by another user
                var existingUser = await _userRepository.GetByEmailAsync(request.Email);
                if (existingUser != null && existingUser.UserId != userId)
                {
                    return false;
                }
                user.Email = request.Email;
            }

            await _userRepository.UpdateAsync(user);
            return true;
        }
    }
}
