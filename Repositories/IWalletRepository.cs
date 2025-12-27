using WalletAPI.Models;

namespace WalletAPI.Repositories
{
    public interface IWalletRepository
    {
        Task<Wallet?> GetByUserIdAsync(int userId);
        Task<Wallet?> GetByIdAsync(int walletId);
        Task<Wallet> CreateAsync(Wallet wallet);
        Task<Wallet> UpdateAsync(Wallet wallet);
        Task<WalletTransaction> CreateTransactionAsync(WalletTransaction transaction);
        Task<List<WalletTransaction>> GetTransactionsAsync(int walletId, int page, int pageSize);
        Task<int> GetTransactionCountAsync(int walletId);
        Task<WalletTopupRequest> CreateTopupRequestAsync(WalletTopupRequest request);
        Task<WalletTopupRequest?> GetTopupRequestByRefNoAsync(string txnRefNo);
        Task<WalletTopupRequest> UpdateTopupRequestAsync(WalletTopupRequest request);
        Task<decimal> GetHoldAmountAsync(int walletId);
        Task<WalletHoldAmount> CreateHoldAsync(WalletHoldAmount hold);
        Task ReleaseHoldAsync(long holdId);
    }
}
