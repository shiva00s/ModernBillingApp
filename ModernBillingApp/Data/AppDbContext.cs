using Microsoft.EntityFrameworkCore;
using ModernBillingApp.Models; // We add this to find the models

namespace ModernBillingApp.Data // This is the new, correct namespace
{
    // This is our new "database connection" and "query service" all in one.
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // These lines tell EF Core about our tables
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerPayment> CustomerPayments { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<StockLedger> StockLedgers { get; set; }
        public DbSet<StockReturn> StockReturns { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillItem> BillItems { get; set; }
        public DbSet<CashEntry> CashEntries { get; set; }
        public DbSet<ShopSettings> ShopSettings { get; set; }

        // Payment and Return Models
        public DbSet<SupplierPayment> SupplierPayments { get; set; }
        public DbSet<CustomerPaymentEnhanced> CustomerPaymentEnhanced { get; set; }
        public DbSet<PurchaseEntry> PurchaseEntries { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }
        public DbSet<PurchaseReturn> PurchaseReturns { get; set; }
        public DbSet<PurchaseReturnItem> PurchaseReturnItems { get; set; }
        public DbSet<BillReturn> BillReturns { get; set; }
        public DbSet<BillReturnItem> BillReturnItems { get; set; }
        public DbSet<LoyaltyTransaction> LoyaltyTransactions { get; set; }
        public DbSet<ExpiryAlert> ExpiryAlerts { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        // Staff Management
        public DbSet<Staff> Staff { get; set; }
        public DbSet<StaffAttendance> StaffAttendances { get; set; }
        public DbSet<StaffSalary> StaffSalaries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure cascade delete behaviors to avoid cycles

            // Disable cascade delete for StockLedger -> Supplier
            modelBuilder.Entity<StockLedger>()
                .HasOne(s => s.Supplier)
                .WithMany()
                .HasForeignKey(s => s.SupplierId)
                .OnDelete(DeleteBehavior.NoAction);

            // Disable cascade delete for PurchaseReturn -> Supplier
            modelBuilder.Entity<PurchaseReturn>()
                .HasOne(pr => pr.Supplier)
                .WithMany()
                .HasForeignKey(pr => pr.SupplierId)
                .OnDelete(DeleteBehavior.NoAction);

            // Disable cascade delete for PurchaseReturn -> PurchaseEntry
            modelBuilder.Entity<PurchaseReturn>()
                .HasOne(pr => pr.PurchaseEntry)
                .WithMany()
                .HasForeignKey(pr => pr.PurchaseEntryId)
                .OnDelete(DeleteBehavior.NoAction);

            // Disable cascade delete for BillReturn -> Bill
            modelBuilder.Entity<BillReturn>()
                .HasOne(br => br.OriginalBill)
                .WithMany()
                .HasForeignKey(br => br.OriginalBillId)
                .OnDelete(DeleteBehavior.NoAction);

            // Disable cascade delete for BillReturn -> Customer
            modelBuilder.Entity<BillReturn>()
                .HasOne(br => br.Customer)
                .WithMany()
                .HasForeignKey(br => br.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);

            // Disable cascade delete for BillReturnItem -> BillItem
            modelBuilder.Entity<BillReturnItem>()
                .HasOne(bri => bri.OriginalBillItem)
                .WithMany()
                .HasForeignKey(bri => bri.OriginalBillItemId)
                .OnDelete(DeleteBehavior.NoAction);

            // Disable cascade delete for BillReturnItem -> Product
            modelBuilder.Entity<BillReturnItem>()
                .HasOne(bri => bri.Product)
                .WithMany()
                .HasForeignKey(bri => bri.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            // Disable cascade delete for PurchaseReturnItem -> PurchaseItem
            modelBuilder.Entity<PurchaseReturnItem>()
                .HasOne(pri => pri.PurchaseItem)
                .WithMany()
                .HasForeignKey(pri => pri.PurchaseItemId)
                .OnDelete(DeleteBehavior.NoAction);

            // Disable cascade delete for PurchaseReturnItem -> Product
            modelBuilder.Entity<PurchaseReturnItem>()
                .HasOne(pri => pri.Product)
                .WithMany()
                .HasForeignKey(pri => pri.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            // Disable cascade delete for PurchaseItem -> Product
            modelBuilder.Entity<PurchaseItem>()
                .HasOne(pi => pi.Product)
                .WithMany()
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            // Disable cascade delete for BillItem -> Product
            modelBuilder.Entity<BillItem>()
                .HasOne(bi => bi.Product)
                .WithMany()
                .HasForeignKey(bi => bi.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            // Disable cascade delete for Bill -> Customer
            modelBuilder.Entity<Bill>()
                .HasOne(b => b.Customer)
                .WithMany(c => c.Bills)
                .HasForeignKey(b => b.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);

            // Disable cascade delete for SupplierPayment -> Supplier
            modelBuilder.Entity<SupplierPayment>()
                .HasOne(sp => sp.Supplier)
                .WithMany()
                .HasForeignKey(sp => sp.SupplierId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure ExpiryAlert relationships (property may not exist, skip if not needed)
            // modelBuilder.Entity<ExpiryAlert>()
            //     .HasOne(ea => ea.AcknowledgedByUser)
            //     .WithMany()
            //     .HasForeignKey(ea => ea.AcknowledgedByUserId)
            //     .OnDelete(DeleteBehavior.NoAction);

            // Configure StaffAttendance relationships (property may not exist, skip if not needed)
            // modelBuilder.Entity<StaffAttendance>()
            //     .HasOne(sa => sa.RecordedByUser)
            //     .WithMany()
            //     .HasForeignKey(sa => sa.RecordedByUserId)
            //     .OnDelete(DeleteBehavior.NoAction);

            // Configure StaffSalary relationships (property may not exist, skip if not needed)
            // modelBuilder.Entity<StaffSalary>()
            //     .HasOne(ss => ss.ProcessedByUser)
            //     .WithMany()
            //     .HasForeignKey(ss => ss.ProcessedByUserId)
            //     .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
