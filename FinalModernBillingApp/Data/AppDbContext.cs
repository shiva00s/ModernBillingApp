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

        // Staff Management Models
        public DbSet<Staff> Staff { get; set; }
        public DbSet<StaffAttendance> StaffAttendances { get; set; }
        public DbSet<StaffSalary> StaffSalaries { get; set; }
        public DbSet<StaffLeave> StaffLeaves { get; set; }
        public DbSet<StaffDocument> StaffDocuments { get; set; }
    }
}
