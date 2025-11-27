using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModernBillingApp.Models
{
    // Role Permission Model - Controls what each role can access
    public class RolePermission
    {
        public int Id { get; set; }
        
        [Required]
        public int UserRoleId { get; set; }
        public UserRole UserRole { get; set; }
        
        [Required]
        [StringLength(100)]
        public string PermissionKey { get; set; } // e.g., "billing.create", "reports.view", "user.manage"
        
        [StringLength(200)]
        public string? Description { get; set; }
        
        public bool IsAllowed { get; set; } = true;
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
    }

    // Permission Keys Constants
    public static class PermissionKeys
    {
        // Billing
        public const string BillingCreate = "billing.create";
        public const string BillingEdit = "billing.edit";
        public const string BillingDelete = "billing.delete";
        public const string BillingView = "billing.view";
        public const string BillingReturn = "billing.return";
        
        // Products
        public const string ProductCreate = "product.create";
        public const string ProductEdit = "product.edit";
        public const string ProductDelete = "product.delete";
        public const string ProductView = "product.view";
        
        // Customers
        public const string CustomerCreate = "customer.create";
        public const string CustomerEdit = "customer.edit";
        public const string CustomerDelete = "customer.delete";
        public const string CustomerView = "customer.view";
        
        // Suppliers
        public const string SupplierCreate = "supplier.create";
        public const string SupplierEdit = "supplier.edit";
        public const string SupplierDelete = "supplier.delete";
        public const string SupplierView = "supplier.view";
        
        // Purchase
        public const string PurchaseCreate = "purchase.create";
        public const string PurchaseEdit = "purchase.edit";
        public const string PurchaseDelete = "purchase.delete";
        public const string PurchaseView = "purchase.view";
        public const string PurchaseReturn = "purchase.return";
        
        // Reports
        public const string ReportSales = "report.sales";
        public const string ReportPurchase = "report.purchase";
        public const string ReportCustomer = "report.customer";
        public const string ReportSupplier = "report.supplier";
        public const string ReportProfit = "report.profit";
        public const string ReportStock = "report.stock";
        
        // Users & Staff
        public const string UserManage = "user.manage";
        public const string UserCreate = "user.create";
        public const string UserEdit = "user.edit";
        public const string UserDelete = "user.delete";
        public const string StaffManage = "staff.manage";
        
        // Settings
        public const string SettingsManage = "settings.manage";
        public const string ShopSettings = "settings.shop";
        
        // Cash Entry
        public const string CashEntryCreate = "cash.create";
        public const string CashEntryEdit = "cash.edit";
        public const string CashEntryDelete = "cash.delete";
        public const string CashEntryView = "cash.view";
        
        // Dashboard
        public const string DashboardView = "dashboard.view";
    }
}

