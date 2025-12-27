using Microsoft.EntityFrameworkCore;
using WalletAPI.Data;
using WalletAPI.Models;

namespace WalletAPI.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly AppDbContext _context;

        public WalletRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Wallet?> GetByUserIdAsync(int userId)
        {
            return await _context.Wallets
                .Include(w => w.User)
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }

        public async Task<Wallet?> GetByIdAsync(int walletId)
        {
            return await _context.Wallets
                .Include(w => w.User)
                .FirstOrDefaultAsync(w => w.WalletId == walletId);
        }

        public async Task<Wallet> CreateAsync(Wallet wallet)
        {
            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();
            return wallet;
        }

        public async Task<Wallet> UpdateAsync(Wallet wallet)
        {
            wallet.LastUpdated = DateTime.Now;
            _context.Wallets.Update(wallet);
            await _context.SaveChangesAsync();
            return wallet;
        }

        public async Task<WalletTransaction> CreateTransactionAsync(WalletTransaction transaction)
        {
            _context.WalletTransactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<List<WalletTransaction>> GetTransactionsAsync(int walletId, int page, int pageSize)
        {
            return await _context.WalletTransactions
                .Where(t => t.WalletId == walletId)
                .OrderByDescending(t => t.CreatedOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTransactionCountAsync(int walletId)
        {
            return await _context.WalletTransactions
                .Where(t => t.WalletId == walletId)
                .CountAsync();
        }

        public async Task<WalletTopupRequest> CreateTopupRequestAsync(WalletTopupRequest request)
        {
            _context.WalletTopupRequests.Add(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<WalletTopupRequest?> GetTopupRequestByRefNoAsync(string txnRefNo)
        {
            return await _context.WalletTopupRequests
                .FirstOrDefaultAsync(t => t.TxnRefNo == txnRefNo);
        }

        public async Task<WalletTopupRequest> UpdateTopupRequestAsync(WalletTopupRequest request)
        {
            _context.WalletTopupRequests.Update(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<decimal> GetHoldAmountAsync(int walletId)
        {
            return await _context.WalletHoldAmounts
                .Where(h => h.WalletId == walletId && !h.IsReleased)
                .SumAsync(h => h.Amount);
        }

        public async Task<WalletHoldAmount> CreateHoldAsync(WalletHoldAmount hold)
        {
            _context.WalletHoldAmounts.Add(hold);
            await _context.SaveChangesAsync();
            return hold;
        }

        public async Task ReleaseHoldAsync(long holdId)
        {
            var hold = await _context.WalletHoldAmounts.FindAsync(holdId);
            if (hold != null)
            {
                hold.IsReleased = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
