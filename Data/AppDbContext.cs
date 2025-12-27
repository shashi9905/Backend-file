using Microsoft.EntityFrameworkCore;
using WalletAPI.Models;

namespace WalletAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WalletTransaction> WalletTransactions { get; set; }
        public DbSet<WalletTopupRequest> WalletTopupRequests { get; set; }
        public DbSet<PaymentWebhookLog> PaymentWebhookLogs { get; set; }
        public DbSet<WalletHoldAmount> WalletHoldAmounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Mobile)
                .IsUnique();

            // Wallet configuration
            modelBuilder.Entity<Wallet>()
                .HasIndex(w => w.UserId);

            modelBuilder.Entity<Wallet>()
                .HasOne(w => w.User)
                .WithOne(u => u.Wallet)
                .HasForeignKey<Wallet>(w => w.UserId);

            // WalletTransaction configuration
            modelBuilder.Entity<WalletTransaction>()
                .HasIndex(t => t.TxnRefNo)
                .IsUnique();

            modelBuilder.Entity<WalletTransaction>()
                .HasIndex(t => t.WalletId);

            modelBuilder.Entity<WalletTransaction>()
                .HasIndex(t => t.CreatedOn);

            // WalletTopupRequest configuration
            modelBuilder.Entity<WalletTopupRequest>()
                .HasIndex(t => t.TxnRefNo)
                .IsUnique();

            // WalletHoldAmount configuration
            modelBuilder.Entity<WalletHoldAmount>()
                .HasOne(h => h.Wallet)
                .WithMany(w => w.HoldAmounts)
                .HasForeignKey(h => h.WalletId);
        }
    }
}
