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
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        /// <summary>
        /// Get wallet balance
        /// </summary>
        [HttpGet("balance")]
        public async Task<ActionResult<ApiResponse<WalletResponse>>> GetBalance()
        {
            var userId = GetUserId();
            var wallet = await _walletService.GetWalletAsync(userId);

            if (wallet == null)
            {
                return NotFound(new ApiResponse<WalletResponse>
                {
                    Success = false,
                    Message = "Wallet not found"
                });
            }

            return Ok(new ApiResponse<WalletResponse>
            {
                Success = true,
                Message = "Wallet retrieved successfully",
                Data = wallet
            });
        }

        /// <summary>
        /// Add money to wallet
        /// </summary>
        [HttpPost("topup")]
        public async Task<ActionResult<TopupResponse>> Topup([FromBody] TopupRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new TopupResponse
                {
                    Success = false,
                    Message = "Invalid request data"
                });
            }

            var userId = GetUserId();
            var response = await _walletService.TopupAsync(userId, request);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Debit from wallet
        /// </summary>
        [HttpPost("debit")]
        public async Task<ActionResult<DebitResponse>> Debit([FromBody] DebitRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new DebitResponse
                {
                    Success = false,
                    Message = "Invalid request data"
                });
            }

            var userId = GetUserId();
            var response = await _walletService.DebitAsync(userId, request);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Get transaction history
        /// </summary>
        [HttpGet("transactions")]
        public async Task<ActionResult<ApiResponse<TransactionListResponse>>> GetTransactions(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var userId = GetUserId();
            var response = await _walletService.GetTransactionsAsync(userId, page, pageSize);

            return Ok(new ApiResponse<TransactionListResponse>
            {
                Success = true,
                Message = "Transactions retrieved successfully",
                Data = response
            });
        }
    }
}
