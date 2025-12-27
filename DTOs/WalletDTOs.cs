using System.ComponentModel.DataAnnotations;

namespace WalletAPI.DTOs
{
    // Wallet DTOs
    public class WalletResponse
    {
        public int WalletId { get; set; }
        public decimal Balance { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal HoldAmount { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class TopupRequest
    {
        [Required]
        [Range(1, 100000)]
        public decimal Amount { get; set; }

        public string? Gateway { get; set; }
    }

    public class TopupResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? TxnRefNo { get; set; }
        public decimal? NewBalance { get; set; }
    }

    public class DebitRequest
    {
        [Required]
        [Range(1, 100000)]
        public decimal Amount { get; set; }

        [MaxLength(255)]
        public string? Narration { get; set; }
    }

    public class DebitResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? TxnRefNo { get; set; }
        public decimal? NewBalance { get; set; }
    }

    public class TransactionResponse
    {
        public long TxnId { get; set; }
        public string TxnRefNo { get; set; } = string.Empty;
        public string TxnType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Narration { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class TransactionListResponse
    {
        public List<TransactionResponse> Transactions { get; set; } = new List<TransactionResponse>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }
}
