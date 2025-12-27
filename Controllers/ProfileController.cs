using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WalletAPI.DTOs;
using WalletAPI.Services;

namespace WalletAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;
        private readonly IAuthService _authService;

        public ProfileController(IProfileService profileService, IAuthService authService)
        {
            _profileService = profileService;
            _authService = authService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        /// <summary>
        /// Get user profile
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<ProfileResponse>>> GetProfile()
        {
            var userId = GetUserId();
            var profile = await _profileService.GetProfileAsync(userId);

            if (profile == null)
            {
                return NotFound(new ApiResponse<ProfileResponse>
                {
                    Success = false,
                    Message = "Profile not found"
                });
            }

            return Ok(new ApiResponse<ProfileResponse>
            {
                Success = true,
                Message = "Profile retrieved successfully",
                Data = profile
            });
        }

        /// <summary>
        /// Update user profile
        /// </summary>
        [HttpPut]
        public async Task<ActionResult<ApiResponse<object>>> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid request data"
                });
            }

            var userId = GetUserId();
            var success = await _profileService.UpdateProfileAsync(userId, request);

            if (!success)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Failed to update profile. Email might already be in use."
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Profile updated successfully"
            });
        }

        /// <summary>
        /// Change password
        /// </summary>
        [HttpPost("change-password")]
        public async Task<ActionResult<ApiResponse<object>>> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid request data"
                });
            }

            var userId = GetUserId();
            var success = await _authService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);

            if (!success)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Failed to change password. Current password is incorrect."
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Password changed successfully"
            });
        }
    }
}
