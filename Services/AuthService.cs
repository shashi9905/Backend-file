using WalletAPI.DTOs;
using WalletAPI.Helpers;
using WalletAPI.Models;
using WalletAPI.Repositories;

namespace WalletAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly JwtHelper _jwtHelper;

        public AuthService(
            IUserRepository userRepository,
            IWalletRepository walletRepository,
            JwtHelper jwtHelper)
        {
            _userRepository = userRepository;
            _walletRepository = walletRepository;
            _jwtHelper = jwtHelper;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByMobileAsync(request.Mobile);

            if (user == null)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Invalid mobile number or password"
                };
            }

            if (!user.IsActive)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Account is deactivated. Please contact support."
                };
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Invalid mobile number or password"
                };
            }

            var token = _jwtHelper.GenerateToken(user);

            return new AuthResponse
            {
                Success = true,
                Message = "Login successful",
                Token = token,
                User = new UserDto
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Mobile = user.Mobile,
                    Email = user.Email,
                    IsActive = user.IsActive,
                    CreatedOn = user.CreatedOn
                }
            };
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            // Check if mobile already exists
            if (await _userRepository.MobileExistsAsync(request.Mobile))
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Mobile number already registered"
                };
            }

            // Check if email already exists (if provided)
            if (!string.IsNullOrEmpty(request.Email) && await _userRepository.EmailExistsAsync(request.Email))
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Email already registered"
                };
            }

            // Create user
            var user = new User
            {
                FullName = request.FullName,
                Mobile = request.Mobile,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                IsActive = true,
                CreatedOn = DateTime.Now
            };

            user = await _userRepository.CreateAsync(user);

            // Create wallet for user
            var wallet = new Wallet
            {
                UserId = user.UserId,
                Balance = 0,
                LastUpdated = DateTime.Now
            };

            await _walletRepository.CreateAsync(wallet);

            var token = _jwtHelper.GenerateToken(user);

            return new AuthResponse
            {
                Success = true,
                Message = "Registration successful",
                Token = token,
                User = new UserDto
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Mobile = user.Mobile,
                    Email = user.Email,
                    IsActive = user.IsActive,
                    CreatedOn = user.CreatedOn
                }
            };
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            {
                return false;
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);

            return true;
        }
    }
}
