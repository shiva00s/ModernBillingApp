using Microsoft.EntityFrameworkCore;
using ModernBillingApp.Data;
using ModernBillingApp.Models;

namespace ModernBillingApp.Services
{
    // Permission Management Service
    public class PermissionService
    {
        private readonly AppDbContext _context;

        public PermissionService(AppDbContext context)
        {
            _context = context;
        }

        // Check if user has permission
        public async Task<bool> HasPermission(int userRoleId, string permissionKey)
        {
            // SuperAdmin has all permissions
            var role = await _context.UserRoles.FindAsync(userRoleId);
            if (role?.RoleName == "SuperAdmin")
                return true;

            var permission = await _context.RolePermissions
                .FirstOrDefaultAsync(p => p.UserRoleId == userRoleId && 
                                         p.PermissionKey == permissionKey);

            return permission?.IsAllowed ?? false;
        }

        // Check if current user has permission (from SessionService)
        public async Task<bool> HasPermission(User? user, string permissionKey)
        {
            if (user == null) return false;
            return await HasPermission(user.UserRoleId, permissionKey);
        }

        // Get all permissions for a role
        public async Task<List<RolePermission>> GetRolePermissions(int userRoleId)
        {
            return await _context.RolePermissions
                .Where(p => p.UserRoleId == userRoleId)
                .ToListAsync();
        }

        // Set permission for a role
        public async Task SetPermission(int userRoleId, string permissionKey, bool isAllowed, string? description = null)
        {
            var existing = await _context.RolePermissions
                .FirstOrDefaultAsync(p => p.UserRoleId == userRoleId && 
                                         p.PermissionKey == permissionKey);

            if (existing != null)
            {
                existing.IsAllowed = isAllowed;
                if (!string.IsNullOrEmpty(description))
                    existing.Description = description;
            }
            else
            {
                _context.RolePermissions.Add(new RolePermission
                {
                    UserRoleId = userRoleId,
                    PermissionKey = permissionKey,
                    IsAllowed = isAllowed,
                    Description = description
                });
            }

            await _context.SaveChangesAsync();
        }

        // Initialize default permissions for roles
        public async Task InitializeDefaultPermissions()
        {
            // Get roles
            var superAdminRole = await _context.UserRoles.FirstOrDefaultAsync(r => r.RoleName == "SuperAdmin");
            var adminRole = await _context.UserRoles.FirstOrDefaultAsync(r => r.RoleName == "Admin");
            var staffRole = await _context.UserRoles.FirstOrDefaultAsync(r => r.RoleName == "Staff");

            if (superAdminRole == null || adminRole == null || staffRole == null)
                return;

            // SuperAdmin - All permissions (handled in HasPermission method)
            
            // Admin - Most permissions except user management
            var adminPermissions = new[]
            {
                PermissionKeys.BillingCreate, PermissionKeys.BillingEdit, PermissionKeys.BillingView,
                PermissionKeys.ProductCreate, PermissionKeys.ProductEdit, PermissionKeys.ProductView,
                PermissionKeys.CustomerCreate, PermissionKeys.CustomerEdit, PermissionKeys.CustomerView,
                PermissionKeys.SupplierCreate, PermissionKeys.SupplierEdit, PermissionKeys.SupplierView,
                PermissionKeys.PurchaseCreate, PermissionKeys.PurchaseEdit, PermissionKeys.PurchaseView,
                PermissionKeys.ReportSales, PermissionKeys.ReportPurchase, PermissionKeys.ReportCustomer,
                PermissionKeys.ReportSupplier, PermissionKeys.ReportProfit, PermissionKeys.ReportStock,
                PermissionKeys.CashEntryCreate, PermissionKeys.CashEntryEdit, PermissionKeys.CashEntryView,
                PermissionKeys.DashboardView, PermissionKeys.ShopSettings
            };

            foreach (var perm in adminPermissions)
            {
                await SetPermission(adminRole.Id, perm, true);
            }

            // Staff - Limited permissions
            var staffPermissions = new[]
            {
                PermissionKeys.BillingCreate, PermissionKeys.BillingView,
                PermissionKeys.ProductView,
                PermissionKeys.CustomerView,
                PermissionKeys.CashEntryCreate, PermissionKeys.CashEntryView,
                PermissionKeys.DashboardView
            };

            foreach (var perm in staffPermissions)
            {
                await SetPermission(staffRole.Id, perm, true);
            }
        }
    }
}

