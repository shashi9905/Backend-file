using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalletAPI.Models
{
    [Table("Wallet")]
    public class Wallet
    {
        [Key]
        public int WalletId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; } = 0;

        public DateTime LastUpdated { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        public virtual ICollection<WalletTransaction> Transactions { get; set; } = new List<WalletTransaction>();
        public virtual ICollection<WalletHoldAmount> HoldAmounts { get; set; } = new List<WalletHoldAmount>();
    }
}
