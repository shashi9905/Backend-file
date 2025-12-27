using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalletAPI.Models
{
    [Table("PaymentWebhookLogs")]
    public class PaymentWebhookLog
    {
        [Key]
        public long LogId { get; set; }

        [MaxLength(50)]
        public string? TxnRefNo { get; set; }

        public string? RawResponse { get; set; }

        [MaxLength(20)]
        public string? Status { get; set; }

        public DateTime ReceivedOn { get; set; } = DateTime.Now;
    }
}
