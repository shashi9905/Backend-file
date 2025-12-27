using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalletAPI.Models
{
    [Table("WalletTopupRequest")]
    public class WalletTopupRequest
    {
        [Key]
        public long RequestId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string TxnRefNo { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [MaxLength(50)]
        public string? Gateway { get; set; } // Razorpay / Cashfree / InstantPay

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = string.Empty; // INITIATED / SUCCESS / FAILED

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
