using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalletAPI.Models
{
    [Table("WalletHoldAmount")]
    public class WalletHoldAmount
    {
        [Key]
        public long HoldId { get; set; }

        [Required]
        public int WalletId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [MaxLength(200)]
        public string? Reason { get; set; }

        public bool IsReleased { get; set; } = false;

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("WalletId")]
        public virtual Wallet? Wallet { get; set; }
    }
}
