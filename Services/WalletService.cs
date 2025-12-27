using WalletAPI.DTOs;
using WalletAPI.Helpers;
using WalletAPI.Models;
using WalletAPI.Repositories;

namespace WalletAPI.Services
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;

        public WalletService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
        }

        public async Task<WalletResponse?> GetWalletAsync(int userId)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(userId);

            if (wallet == null)
            {
                return null;
            }

            var holdAmount = await _walletRepository.GetHoldAmountAsync(wallet.WalletId);

            return new WalletResponse
            {
                WalletId = wallet.WalletId,
                Balance = wallet.Balance,
                AvailableBalance = wallet.Balance - holdAmount,
                HoldAmount = holdAmount,
                LastUpdated = wallet.LastUpdated
            };
        }

        public async Task<TopupResponse> TopupAsync(int userId, TopupRequest request)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(userId);

            if (wallet == null)
            {
                return new TopupResponse
                {
                    Success = false,
                    Message = "Wallet not found"
                };
            }

            var txnRefNo = TransactionHelper.GenerateTopupRefNo();
            var openingBalance = wallet.Balance;
            var closingBalance = openingBalance + request.Amount;

            // Create topup request
            var topupRequest = new WalletTopupRequest
            {
                UserId = userId,
                TxnRefNo = txnRefNo,
                Amount = request.Amount,
                Gateway = request.Gateway ?? "Direct",
                Status = "SUCCESS",
                CreatedOn = DateTime.Now
            };

            await _walletRepository.CreateTopupRequestAsync(topupRequest);

            // Update wallet balance
            wallet.Balance = closingBalance;
            await _walletRepository.UpdateAsync(wallet);

            // Create transaction record
            var transaction = new WalletTransaction
            {
                WalletId = wallet.WalletId,
                TxnRefNo = txnRefNo,
                TxnType = "CREDIT",
                Amount = request.Amount,
                OpeningBalance = openingBalance,
                ClosingBalance = closingBalance,
                Status = "SUCCESS",
                Narration = $"Wallet topup via {request.Gateway ?? "Direct"}",
                CreatedOn = DateTime.Now
            };

            await _walletRepository.CreateTransactionAsync(transaction);

            return new TopupResponse
            {
                Success = true,
                Message = "Topup successful",
                TxnRefNo = txnRefNo,
                NewBalance = closingBalance
            };
        }

        public async Task<DebitResponse> DebitAsync(int userId, DebitRequest request)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(userId);

            if (wallet == null)
            {
                return new DebitResponse
                {
                    Success = false,
                    Message = "Wallet not found"
                };
            }

            var holdAmount = await _walletRepository.GetHoldAmountAsync(wallet.WalletId);
            var availableBalance = wallet.Balance - holdAmount;

            if (request.Amount > availableBalance)
            {
                return new DebitResponse
                {
                    Success = false,
                    Message = "Insufficient balance"
                };
            }

            var txnRefNo = TransactionHelper.GenerateDebitRefNo();
            var openingBalance = wallet.Balance;
            var closingBalance = openingBalance - request.Amount;

            // Update wallet balance
            wallet.Balance = closingBalance;
            await _walletRepository.UpdateAsync(wallet);

            // Create transaction record
            var transaction = new WalletTransaction
            {
                WalletId = wallet.WalletId,
                TxnRefNo = txnRefNo,
                TxnType = "DEBIT",
                Amount = request.Amount,
                OpeningBalance = openingBalance,
                ClosingBalance = closingBalance,
                Status = "SUCCESS",
                Narration = request.Narration ?? "Wallet debit",
                CreatedOn = DateTime.Now
            };

            await _walletRepository.CreateTransactionAsync(transaction);

            return new DebitResponse
            {
                Success = true,
                Message = "Debit successful",
                TxnRefNo = txnRefNo,
                NewBalance = closingBalance
            };
        }

        public async Task<TransactionListResponse> GetTransactionsAsync(int userId, int page, int pageSize)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(userId);

            if (wallet == null)
            {
                return new TransactionListResponse
                {
                    Transactions = new List<TransactionResponse>(),
                    TotalCount = 0,
                    Page = page,
                    PageSize = pageSize
                };
            }

            var transactions = await _walletRepository.GetTransactionsAsync(wallet.WalletId, page, pageSize);
            var totalCount = await _walletRepository.GetTransactionCountAsync(wallet.WalletId);

            return new TransactionListResponse
            {
                Transactions = transactions.Select(t => new TransactionResponse
                {
                    TxnId = t.TxnId,
                    TxnRefNo = t.TxnRefNo,
                    TxnType = t.TxnType,
                    Amount = t.Amount,
                    OpeningBalance = t.OpeningBalance,
                    ClosingBalance = t.ClosingBalance,
                    Status = t.Status,
                    Narration = t.Narration,
                    CreatedOn = t.CreatedOn
                }).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
