using WalletAPI.DTOs;

namespace WalletAPI.Services
{
    public interface IWalletService
    {
        Task<WalletResponse?> GetWalletAsync(int userId);
        Task<TopupResponse> TopupAsync(int userId, TopupRequest request);
        Task<DebitResponse> DebitAsync(int userId, DebitRequest request);
        Task<TransactionListResponse> GetTransactionsAsync(int userId, int page, int pageSize);
    }
}
