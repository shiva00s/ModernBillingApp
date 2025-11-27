using Microsoft.EntityFrameworkCore;
using NewModernBillingApp.Models;

namespace NewModernBillingApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // User Management
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserPreference> UserPreferences { get; set; }

        // Staff Management
        public DbSet<Staff> Staff { get; set; }
        public DbSet<StaffAttendance> StaffAttendances { get; set; }
        public DbSet<StaffSalary> StaffSalaries { get; set; }
        public DbSet<StaffLeave> StaffLeaves { get; set; }
        public DbSet<StaffDocument> StaffDocuments { get; set; }

        // Customer Management
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerPayment> CustomerPayments { get; set; }

        // Product & Supplier Management
        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<SupplierPayment> SupplierPayments { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }
        public DbSet<StockLedger> StockLedgers { get; set; }

        // Billing
        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillItem> BillItems { get; set; }
        public DbSet<BillReturn> BillReturns { get; set; }
        public DbSet<BillReturnItem> BillReturnItems { get; set; }

        // Cash Management
        public DbSet<CashEntry> CashEntries { get; set; }

        // System
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationTemplate> NotificationTemplates { get; set; }
        public DbSet<AppSettings> AppSettings { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<CompanyInfo> CompanyInfo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configurations
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasOne(e => e.Role)
                      .WithMany(r => r.Users)
                      .HasForeignKey(e => e.RoleId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.RoleName).IsUnique();
            });

            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Role)
                      .WithMany(r => r.Permissions)
                      .HasForeignKey(e => e.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => new { e.RoleId, e.PermissionKey }).IsUnique();
            });

            // Staff configurations
            modelBuilder.Entity<Staff>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.EmployeeId).IsUnique();
                entity.HasOne(e => e.User)
                      .WithOne()
                      .HasForeignKey<Staff>(e => e.UserId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<StaffAttendance>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Staff)
                      .WithMany(s => s.Attendances)
                      .HasForeignKey(e => e.StaffId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => new { e.StaffId, e.AttendanceDate }).IsUnique();
            });

            modelBuilder.Entity<StaffSalary>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Staff)
                      .WithMany(s => s.Salaries)
                      .HasForeignKey(e => e.StaffId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.ProcessedBy)
                      .WithMany()
                      .HasForeignKey(e => e.ProcessedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => new { e.StaffId, e.SalaryMonth }).IsUnique();
            });

            modelBuilder.Entity<StaffLeave>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Staff)
                      .WithMany(s => s.Leaves)
                      .HasForeignKey(e => e.StaffId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.ApprovedBy)
                      .WithMany()
                      .HasForeignKey(e => e.ApprovedByUserId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Customer configurations
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Phone);
                entity.HasIndex(e => e.Email);
            });

            modelBuilder.Entity<CustomerPayment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Customer)
                      .WithMany(c => c.Payments)
                      .HasForeignKey(e => e.CustomerId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Bill)
                      .WithMany(b => b.Payments)
                      .HasForeignKey(e => e.BillId)
                      .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(e => e.CreatedBy)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Product & Supplier configurations
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.ProductCode);
                entity.HasIndex(e => e.Barcode);
                entity.HasOne(e => e.Supplier)
                      .WithMany(s => s.Products)
                      .HasForeignKey(e => e.SupplierId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Phone);
                entity.HasIndex(e => e.Email);
                entity.HasIndex(e => e.GSTNumber);
            });

            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.PurchaseNo).IsUnique();
                entity.HasOne(e => e.Supplier)
                      .WithMany(s => s.Purchases)
                      .HasForeignKey(e => e.SupplierId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.CreatedBy)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PurchaseItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Purchase)
                      .WithMany(p => p.Items)
                      .HasForeignKey(e => e.PurchaseId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Product)
                      .WithMany(p => p.PurchaseItems)
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<StockLedger>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Product)
                      .WithMany(p => p.StockLedgers)
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.CreatedBy)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<SupplierPayment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Supplier)
                      .WithMany(s => s.Payments)
                      .HasForeignKey(e => e.SupplierId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Purchase)
                      .WithMany(p => p.Payments)
                      .HasForeignKey(e => e.PurchaseId)
                      .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(e => e.CreatedBy)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Bill configurations
            modelBuilder.Entity<Bill>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.BillNo).IsUnique();
                entity.HasOne(e => e.Customer)
                      .WithMany(c => c.Bills)
                      .HasForeignKey(e => e.CustomerId)
                      .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(e => e.CreatedBy)
                      .WithMany(u => u.Bills)
                      .HasForeignKey(e => e.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<BillItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Bill)
                      .WithMany(b => b.Items)
                      .HasForeignKey(e => e.BillId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Product)
                      .WithMany(p => p.BillItems)
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<BillReturn>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.ReturnNo).IsUnique();
                entity.HasOne(e => e.OriginalBill)
                      .WithMany()
                      .HasForeignKey(e => e.OriginalBillId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Customer)
                      .WithMany()
                      .HasForeignKey(e => e.CustomerId)
                      .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(e => e.CreatedBy)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<BillReturnItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.BillReturn)
                      .WithMany(br => br.Items)
                      .HasForeignKey(e => e.BillReturnId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.OriginalBillItem)
                      .WithMany()
                      .HasForeignKey(e => e.OriginalBillItemId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Product)
                      .WithMany()
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Cash Entry configurations
            modelBuilder.Entity<CashEntry>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Customer)
                      .WithMany()
                      .HasForeignKey(e => e.CustomerId)
                      .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(e => e.Supplier)
                      .WithMany()
                      .HasForeignKey(e => e.SupplierId)
                      .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(e => e.CreatedBy)
                      .WithMany(u => u.CashEntries)
                      .HasForeignKey(e => e.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Notification configurations
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Role)
                      .WithMany()
                      .HasForeignKey(e => e.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.CreatedBy)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // System configurations
            modelBuilder.Entity<AppSettings>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.SettingKey).IsUnique();
                entity.HasOne(e => e.UpdatedBy)
                      .WithMany()
                      .HasForeignKey(e => e.UpdatedByUserId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<UserPreference>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => new { e.UserId, e.PreferenceKey }).IsUnique();
            });

            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.SetNull);
                entity.HasIndex(e => e.Timestamp);
                entity.HasIndex(e => new { e.EntityType, e.EntityId });
            });

            modelBuilder.Entity<CompanyInfo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.UpdatedBy)
                      .WithMany()
                      .HasForeignKey(e => e.UpdatedByUserId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<StaffDocument>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Staff)
                      .WithMany()
                      .HasForeignKey(e => e.StaffId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.UploadedBy)
                      .WithMany()
                      .HasForeignKey(e => e.UploadedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed default roles
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { Id = 1, RoleName = "SuperAdmin", Description = "Super Administrator with full access", IsSystemRole = true },
                new UserRole { Id = 2, RoleName = "Admin", Description = "Administrator with limited access", IsSystemRole = true },
                new UserRole { Id = 3, RoleName = "Staff", Description = "Staff member with basic access", IsSystemRole = true },
                new UserRole { Id = 4, RoleName = "Employee", Description = "Employee with minimal access", IsSystemRole = true }
            );

            // Seed default permissions for SuperAdmin (all permissions)
            var superAdminPermissions = new[]
            {
                new RolePermission { Id = 1, RoleId = 1, PermissionKey = "Dashboard", CanRead = true, CanCreate = true, CanUpdate = true, CanDelete = true },
                new RolePermission { Id = 2, RoleId = 1, PermissionKey = "UserManagement", CanRead = true, CanCreate = true, CanUpdate = true, CanDelete = true },
                new RolePermission { Id = 3, RoleId = 1, PermissionKey = "StaffManagement", CanRead = true, CanCreate = true, CanUpdate = true, CanDelete = true },
                new RolePermission { Id = 4, RoleId = 1, PermissionKey = "CustomerManagement", CanRead = true, CanCreate = true, CanUpdate = true, CanDelete = true },
                new RolePermission { Id = 5, RoleId = 1, PermissionKey = "SupplierManagement", CanRead = true, CanCreate = true, CanUpdate = true, CanDelete = true },
                new RolePermission { Id = 6, RoleId = 1, PermissionKey = "ProductManagement", CanRead = true, CanCreate = true, CanUpdate = true, CanDelete = true },
                new RolePermission { Id = 7, RoleId = 1, PermissionKey = "Billing", CanRead = true, CanCreate = true, CanUpdate = true, CanDelete = true },
                new RolePermission { Id = 8, RoleId = 1, PermissionKey = "Purchase", CanRead = true, CanCreate = true, CanUpdate = true, CanDelete = true },
                new RolePermission { Id = 9, RoleId = 1, PermissionKey = "CashEntry", CanRead = true, CanCreate = true, CanUpdate = true, CanDelete = true },
                new RolePermission { Id = 10, RoleId = 1, PermissionKey = "Reports", CanRead = true, CanCreate = true, CanUpdate = true, CanDelete = true },
                new RolePermission { Id = 11, RoleId = 1, PermissionKey = "Settings", CanRead = true, CanCreate = true, CanUpdate = true, CanDelete = true },
                new RolePermission { Id = 12, RoleId = 1, PermissionKey = "Notifications", CanRead = true, CanCreate = true, CanUpdate = true, CanDelete = true }
            };

            // Seed default permissions for Admin
            var adminPermissions = new[]
            {
                new RolePermission { Id = 13, RoleId = 2, PermissionKey = "Dashboard", CanRead = true, CanCreate = false, CanUpdate = false, CanDelete = false },
                new RolePermission { Id = 14, RoleId = 2, PermissionKey = "StaffManagement", CanRead = true, CanCreate = true, CanUpdate = true, CanDelete = false },
                new RolePermission { Id = 15, RoleId = 2, PermissionKey = "CustomerManagement", CanRead = true, CanCreate = true, CanUpdate = true, CanDelete = true },
                new RolePermission { Id = 16, RoleId = 2, PermissionKey = "SupplierManagement", CanRead = true, CanCreate = true, CanUpdate = true, CanDelete = true },
                new RolePermission { Id = 17, RoleId = 2, PermissionKey = "ProductManagement", CanRead = true, CanCreate = true, CanUpdate = true, CanDelete = true },
                new RolePermission { Id = 18, RoleId = 2, PermissionKey = "Billing", CanRead = true, CanCreate = true, CanUpdate = true, CanDelete = false },
                new RolePermission { Id = 19, RoleId = 2, PermissionKey = "Purchase", CanRead = true, CanCreate = true, CanUpdate = true, CanDelete = false },
                new RolePermission { Id = 20, RoleId = 2, PermissionKey = "CashEntry", CanRead = true, CanCreate = true, CanUpdate = true, CanDelete = false },
                new RolePermission { Id = 21, RoleId = 2, PermissionKey = "Reports", CanRead = true, CanCreate = false, CanUpdate = false, CanDelete = false },
                new RolePermission { Id = 22, RoleId = 2, PermissionKey = "Notifications", CanRead = true, CanCreate = false, CanUpdate = false, CanDelete = false }
            };

            // Seed default permissions for Staff
            var staffPermissions = new[]
            {
                new RolePermission { Id = 23, RoleId = 3, PermissionKey = "Dashboard", CanRead = true, CanCreate = false, CanUpdate = false, CanDelete = false },
                new RolePermission { Id = 24, RoleId = 3, PermissionKey = "CustomerManagement", CanRead = true, CanCreate = true, CanUpdate = true, CanDelete = false },
                new RolePermission { Id = 25, RoleId = 3, PermissionKey = "ProductManagement", CanRead = true, CanCreate = false, CanUpdate = false, CanDelete = false },
                new RolePermission { Id = 26, RoleId = 3, PermissionKey = "Billing", CanRead = true, CanCreate = true, CanUpdate = false, CanDelete = false },
                new RolePermission { Id = 27, RoleId = 3, PermissionKey = "CashEntry", CanRead = true, CanCreate = true, CanUpdate = false, CanDelete = false }
            };

            // Seed default permissions for Employee
            var employeePermissions = new[]
            {
                new RolePermission { Id = 28, RoleId = 4, PermissionKey = "Dashboard", CanRead = true, CanCreate = false, CanUpdate = false, CanDelete = false },
                new RolePermission { Id = 29, RoleId = 4, PermissionKey = "Billing", CanRead = true, CanCreate = true, CanUpdate = false, CanDelete = false }
            };

            modelBuilder.Entity<RolePermission>().HasData(superAdminPermissions);
            modelBuilder.Entity<RolePermission>().HasData(adminPermissions);
            modelBuilder.Entity<RolePermission>().HasData(staffPermissions);
            modelBuilder.Entity<RolePermission>().HasData(employeePermissions);

            // Seed default app settings
            modelBuilder.Entity<AppSettings>().HasData(
                new AppSettings { Id = 1, SettingKey = "CompanyName", SettingValue = "Modern Billing App", Category = "Company", DisplayName = "Company Name", IsSystemSetting = false },
                new AppSettings { Id = 2, SettingKey = "Currency", SettingValue = "INR", Category = "General", DisplayName = "Currency", IsSystemSetting = false },
                new AppSettings { Id = 3, SettingKey = "CurrencySymbol", SettingValue = "₹", Category = "General", DisplayName = "Currency Symbol", IsSystemSetting = false },
                new AppSettings { Id = 4, SettingKey = "DefaultTheme", SettingValue = "light", Category = "UI", DisplayName = "Default Theme", IsSystemSetting = false },
                new AppSettings { Id = 5, SettingKey = "ItemsPerPage", SettingValue = "25", Category = "UI", DisplayName = "Items Per Page", IsSystemSetting = false },
                new AppSettings { Id = 6, SettingKey = "AutoBackup", SettingValue = "true", Category = "System", DisplayName = "Enable Auto Backup", IsSystemSetting = true },
                new AppSettings { Id = 7, SettingKey = "BackupInterval", SettingValue = "24", Category = "System", DisplayName = "Backup Interval (Hours)", IsSystemSetting = true },
                new AppSettings { Id = 8, SettingKey = "WhatsAppAPI", SettingValue = "", Category = "Integration", DisplayName = "WhatsApp API Key", IsSystemSetting = false },
                new AppSettings { Id = 9, SettingKey = "SMSGateway", SettingValue = "", Category = "Integration", DisplayName = "SMS Gateway API", IsSystemSetting = false },
                new AppSettings { Id = 10, SettingKey = "LowStockAlert", SettingValue = "10", Category = "Inventory", DisplayName = "Low Stock Alert Level", IsSystemSetting = false }
            );
        }
    }
}
