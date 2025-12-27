using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalletAPI.Models
{
    [Table("WalletTransactions")]
    public class WalletTransaction
    {
        [Key]
        public long TxnId { get; set; }

        [Required]
        public int WalletId { get; set; }

        [Required]
        [MaxLength(50)]
        public string TxnRefNo { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string TxnType { get; set; } = string.Empty; // CREDIT / DEBIT

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal OpeningBalance { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ClosingBalance { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = string.Empty; // SUCCESS / FAILED / PENDING

        [MaxLength(255)]
        public string? Narration { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("WalletId")]
        public virtual Wallet? Wallet { get; set; }
    }
}
