using Microsoft.EntityFrameworkCore;
using NewModernBillingApp.Data;
using NewModernBillingApp.Models;

namespace NewModernBillingApp.Services
{
    public class CashEntryService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CashEntryService> _logger;
        private readonly SessionService _sessionService;

        public CashEntryService(AppDbContext context, ILogger<CashEntryService> logger, SessionService sessionService)
        {
            _context = context;
            _logger = logger;
            _sessionService = sessionService;
        }

        public async Task<List<CashEntry>> GetAllCashEntriesAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var query = _context.CashEntries
                    .Include(c => c.Customer)
                    .Include(c => c.Supplier)
                    .Include(c => c.CreatedBy)
                    .Where(c => c.IsActive);

                if (fromDate.HasValue)
                    query = query.Where(c => c.EntryDate >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(c => c.EntryDate <= toDate.Value);

                return await query
                    .OrderByDescending(c => c.EntryDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all cash entries");
                return new List<CashEntry>();
            }
        }

        public async Task<CashEntry?> GetCashEntryByIdAsync(int entryId)
        {
            try
            {
                return await _context.CashEntries
                    .Include(c => c.Customer)
                    .Include(c => c.Supplier)
                    .Include(c => c.CreatedBy)
                    .FirstOrDefaultAsync(c => c.Id == entryId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cash entry with ID '{EntryId}'", entryId);
                return null;
            }
        }

        public async Task<bool> CreateCashEntryAsync(CashEntry cashEntry)
        {
            try
            {
                cashEntry.CreatedByUserId = _sessionService.CurrentUserId ?? 1;
                cashEntry.CreatedDate = DateTime.Now;
                cashEntry.IsActive = true;

                _context.CashEntries.Add(cashEntry);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Cash entry '{EntryName}' created successfully with amount {Amount}",
                    cashEntry.EntryName, cashEntry.Amount);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating cash entry '{EntryName}'", cashEntry.EntryName);
                return false;
            }
        }

        public async Task<bool> UpdateCashEntryAsync(CashEntry cashEntry)
        {
            try
            {
                var existingEntry = await _context.CashEntries.FindAsync(cashEntry.Id);
                if (existingEntry == null)
                {
                    _logger.LogWarning("Cash entry update failed: Entry with ID '{EntryId}' not found", cashEntry.Id);
                    return false;
                }

                existingEntry.EntryName = cashEntry.EntryName;
                existingEntry.EntryType = cashEntry.EntryType;
                existingEntry.Category = cashEntry.Category;
                existingEntry.Amount = cashEntry.Amount;
                existingEntry.EntryDate = cashEntry.EntryDate;
                existingEntry.Description = cashEntry.Description;
                existingEntry.PaymentMode = cashEntry.PaymentMode;
                existingEntry.TransactionReference = cashEntry.TransactionReference;
                existingEntry.CustomerId = cashEntry.CustomerId;
                existingEntry.SupplierId = cashEntry.SupplierId;
                existingEntry.ReceivedFrom = cashEntry.ReceivedFrom;
                existingEntry.PaidTo = cashEntry.PaidTo;
                existingEntry.Notes = cashEntry.Notes;
                existingEntry.LastUpdated = DateTime.Now;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Cash entry '{EntryName}' updated successfully", cashEntry.EntryName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cash entry with ID '{EntryId}'", cashEntry.Id);
                return false;
            }
        }

        public async Task<bool> DeleteCashEntryAsync(int entryId)
        {
            try
            {
                var cashEntry = await _context.CashEntries.FindAsync(entryId);
                if (cashEntry == null)
                {
                    _logger.LogWarning("Cash entry deletion failed: Entry with ID '{EntryId}' not found", entryId);
                    return false;
                }

                cashEntry.IsActive = false;
                cashEntry.LastUpdated = DateTime.Now;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Cash entry '{EntryName}' deleted successfully", cashEntry.EntryName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting cash entry with ID '{EntryId}'", entryId);
                return false;
            }
        }

        public async Task<List<CashEntry>> GetCashEntriesByTypeAsync(string entryType, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var query = _context.CashEntries
                    .Include(c => c.Customer)
                    .Include(c => c.Supplier)
                    .Include(c => c.CreatedBy)
                    .Where(c => c.IsActive && c.EntryType == entryType);

                if (fromDate.HasValue)
                    query = query.Where(c => c.EntryDate >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(c => c.EntryDate <= toDate.Value);

                return await query
                    .OrderByDescending(c => c.EntryDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cash entries by type '{EntryType}'", entryType);
                return new List<CashEntry>();
            }
        }

        public async Task<Dictionary<string, decimal>> GetCashSummaryAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var entries = await _context.CashEntries
                    .Where(c => c.IsActive &&
                           c.EntryDate >= fromDate &&
                           c.EntryDate <= toDate)
                    .ToListAsync();

                var income = entries.Where(c => c.EntryType == "Income").Sum(c => c.Amount);
                var expense = entries.Where(c => c.EntryType == "Expense").Sum(c => c.Amount);

                return new Dictionary<string, decimal>
                {
                    {"TotalIncome", income},
                    {"TotalExpense", expense},
                    {"NetCashFlow", income - expense},
                    {"CashEntries", entries.Count}
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cash summary");
                return new Dictionary<string, decimal>
                {
                    {"TotalIncome", 0},
                    {"TotalExpense", 0},
                    {"NetCashFlow", 0},
                    {"CashEntries", 0}
                };
            }
        }

        public async Task<List<string>> GetCategoriesAsync()
        {
            try
            {
                return await _context.CashEntries
                    .Where(c => c.IsActive && !string.IsNullOrEmpty(c.Category))
                    .Select(c => c.Category!)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cash entry categories");
                return new List<string>();
            }
        }

        public async Task<List<CashEntry>> SearchCashEntriesAsync(string searchTerm, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                searchTerm = searchTerm.ToLower();

                var query = _context.CashEntries
                    .Include(c => c.Customer)
                    .Include(c => c.Supplier)
                    .Include(c => c.CreatedBy)
                    .Where(c => c.IsActive &&
                        (c.EntryName.ToLower().Contains(searchTerm) ||
                         c.Description!.ToLower().Contains(searchTerm) ||
                         c.Category!.ToLower().Contains(searchTerm) ||
                         c.ReceivedFrom!.ToLower().Contains(searchTerm) ||
                         c.PaidTo!.ToLower().Contains(searchTerm)));

                if (fromDate.HasValue)
                    query = query.Where(c => c.EntryDate >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(c => c.EntryDate <= toDate.Value);

                return await query
                    .OrderByDescending(c => c.EntryDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching cash entries with term '{SearchTerm}'", searchTerm);
                return new List<CashEntry>();
            }
        }
    }

    public class NotificationService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<NotificationService> _logger;
        private readonly SessionService _sessionService;

        public NotificationService(AppDbContext context, ILogger<NotificationService> logger, SessionService sessionService)
        {
            _context = context;
            _logger = logger;
            _sessionService = sessionService;
        }

        public async Task<List<Notification>> GetUserNotificationsAsync(int userId, bool unreadOnly = false)
        {
            try
            {
                var query = _context.Notifications
                    .Include(n => n.CreatedBy)
                    .Where(n => n.IsActive &&
                           (n.UserId == userId || n.UserId == null) &&
                           (n.ExpiryDate == null || n.ExpiryDate > DateTime.Now));

                if (unreadOnly)
                    query = query.Where(n => !n.IsRead);

                return await query
                    .OrderByDescending(n => n.CreatedDate)
                    .Take(50)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications for user '{UserId}'", userId);
                return new List<Notification>();
            }
        }

        public async Task<int> GetUnreadNotificationCountAsync(int userId)
        {
            try
            {
                return await _context.Notifications
                    .CountAsync(n => n.IsActive &&
                               !n.IsRead &&
                               (n.UserId == userId || n.UserId == null) &&
                               (n.ExpiryDate == null || n.ExpiryDate > DateTime.Now));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread notification count for user '{UserId}'", userId);
                return 0;
            }
        }

        public async Task<bool> CreateNotificationAsync(string title, string message, string type = "Info", string? category = null, int? userId = null, int? roleId = null, string priority = "Normal", string? actionUrl = null, string? actionText = null, bool showToast = false, DateTime? scheduledFor = null, DateTime? expiryDate = null)
        {
            try
            {
                var notification = new Notification
                {
                    Title = title,
                    Message = message,
                    Type = type,
                    Category = category,
                    Priority = priority,
                    UserId = userId,
                    RoleId = roleId,
                    ActionUrl = actionUrl,
                    ActionText = actionText,
                    ShowToast = showToast,
                    ScheduledFor = scheduledFor,
                    ExpiryDate = expiryDate,
                    CreatedByUserId = _sessionService.CurrentUserId,
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    IsSystem = _sessionService.CurrentUserId == null
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Notification '{Title}' created successfully", title);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification '{Title}'", title);
                return false;
            }
        }

        public async Task<bool> MarkAsReadAsync(int notificationId, int userId)
        {
            try
            {
                var notification = await _context.Notifications
                    .FirstOrDefaultAsync(n => n.Id == notificationId &&
                                            (n.UserId == userId || n.UserId == null));

                if (notification == null)
                {
                    _logger.LogWarning("Notification mark as read failed: Notification '{NotificationId}' not found for user '{UserId}'", notificationId, userId);
                    return false;
                }

                notification.IsRead = true;
                notification.ReadDate = DateTime.Now;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification '{NotificationId}' as read for user '{UserId}'", notificationId, userId);
                return false;
            }
        }

        public async Task<bool> MarkAllAsReadAsync(int userId)
        {
            try
            {
                var notifications = await _context.Notifications
                    .Where(n => !n.IsRead &&
                           (n.UserId == userId || n.UserId == null))
                    .ToListAsync();

                foreach (var notification in notifications)
                {
                    notification.IsRead = true;
                    notification.ReadDate = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("All notifications marked as read for user '{UserId}'", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read for user '{UserId}'", userId);
                return false;
            }
        }

        public async Task<bool> DeleteNotificationAsync(int notificationId)
        {
            try
            {
                var notification = await _context.Notifications.FindAsync(notificationId);
                if (notification == null)
                {
                    _logger.LogWarning("Notification deletion failed: Notification '{NotificationId}' not found", notificationId);
                    return false;
                }

                notification.IsActive = false;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Notification '{NotificationId}' deleted successfully", notificationId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification '{NotificationId}'", notificationId);
                return false;
            }
        }

        public async Task CreateSystemNotificationAsync(string title, string message, string type = "System", string? category = null, int? roleId = null)
        {
            try
            {
                await CreateNotificationAsync(title, message, type, category, null, roleId, "Normal");
                _logger.LogInformation("System notification '{Title}' created", title);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating system notification '{Title}'", title);
            }
        }

        public async Task NotifyLowStockAsync()
        {
            try
            {
                var lowStockProducts = await _context.Products
                    .Where(p => p.IsActive && p.CurrentStock <= p.MinimumStock)
                    .ToListAsync();

                if (lowStockProducts.Any())
                {
                    var message = $"Warning: {lowStockProducts.Count} product(s) are running low on stock.";
                    await CreateSystemNotificationAsync("Low Stock Alert", message, "Warning", "Stock", 1); // Admin role
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating low stock notifications");
            }
        }

        public async Task NotifyExpiringProductsAsync()
        {
            try
            {
                var expiringProducts = await _context.Products
                    .Where(p => p.IsActive &&
                           p.ExpiryDate.HasValue &&
                           p.ExpiryDate.Value <= DateTime.Now.AddDays(30) &&
                           p.CurrentStock > 0)
                    .ToListAsync();

                if (expiringProducts.Any())
                {
                    var message = $"Alert: {expiringProducts.Count} product(s) are expiring within 30 days.";
                    await CreateSystemNotificationAsync("Product Expiry Alert", message, "Warning", "Expiry", 1);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating expiry notifications");
            }
        }
    }

    public class ReportService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ReportService> _logger;

        public ReportService(AppDbContext context, ILogger<ReportService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Dictionary<string, object>> GetSalesReportAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var bills = await _context.Bills
                    .Include(b => b.Items)
                    .Include(b => b.Customer)
                    .Where(b => b.IsActive &&
                           b.BillDate >= fromDate &&
                           b.BillDate <= toDate)
                    .ToListAsync();

                var totalSales = bills.Sum(b => b.TotalAmount);
                var totalTax = bills.Sum(b => b.GSTAmount);
                var totalDiscount = bills.Sum(b => b.DiscountAmount);
                var totalProfit = bills.SelectMany(b => b.Items)
                    .Sum(i => (i.Rate - (i.Product?.PurchasePrice ?? 0)) * i.Quantity);

                return new Dictionary<string, object>
                {
                    {"TotalBills", bills.Count},
                    {"TotalSales", totalSales},
                    {"TotalTax", totalTax},
                    {"TotalDiscount", totalDiscount},
                    {"TotalProfit", totalProfit},
                    {"AverageBillValue", bills.Any() ? totalSales / bills.Count : 0},
                    {"CashSales", bills.Where(b => b.PaymentMode == "Cash").Sum(b => b.TotalAmount)},
                    {"CreditSales", bills.Where(b => b.PaymentMode == "Credit").Sum(b => b.TotalAmount)},
                    {"CardSales", bills.Where(b => b.PaymentMode == "Card").Sum(b => b.TotalAmount)},
                    {"UpiSales", bills.Where(b => b.PaymentMode == "UPI").Sum(b => b.TotalAmount)}
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating sales report");
                return new Dictionary<string, object>();
            }
        }

        public async Task<Dictionary<string, object>> GetPurchaseReportAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var purchases = await _context.Purchases
                    .Include(p => p.Items)
                    .Include(p => p.Supplier)
                    .Where(p => p.PurchaseDate >= fromDate &&
                           p.PurchaseDate <= toDate)
                    .ToListAsync();

                return new Dictionary<string, object>
                {
                    {"TotalPurchases", purchases.Count},
                    {"TotalAmount", purchases.Sum(p => p.TotalAmount)},
                    {"TotalTax", purchases.Sum(p => p.GSTAmount)},
                    {"PaidAmount", purchases.Sum(p => p.PaidAmount)},
                    {"PendingAmount", purchases.Sum(p => p.BalanceAmount)},
                    {"AveragePurchaseValue", purchases.Any() ? purchases.Sum(p => p.TotalAmount) / purchases.Count : 0}
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating purchase report");
                return new Dictionary<string, object>();
            }
        }

        public async Task<List<object>> GetTopSellingProductsAsync(DateTime fromDate, DateTime toDate, int count = 10)
        {
            try
            {
                var topProducts = await _context.BillItems
                    .Include(bi => bi.Product)
                    .Include(bi => bi.Bill)
                    .Where(bi => bi.Bill.IsActive &&
                            bi.Bill.BillDate >= fromDate &&
                            bi.Bill.BillDate <= toDate)
                    .GroupBy(bi => new { bi.ProductId, bi.Product.Name })
                    .Select(g => new
                    {
                        ProductId = g.Key.ProductId,
                        ProductName = g.Key.Name,
                        TotalQuantity = g.Sum(bi => bi.Quantity),
                        TotalAmount = g.Sum(bi => bi.Total),
                        TotalSales = g.Count()
                    })
                    .OrderByDescending(p => p.TotalQuantity)
                    .Take(count)
                    .ToListAsync();

                return topProducts.Cast<object>().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top selling products");
                return new List<object>();
            }
        }

        public async Task<List<object>> GetCustomerReportAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var customerReport = await _context.Bills
                    .Include(b => b.Customer)
                    .Where(b => b.IsActive &&
                           b.BillDate >= fromDate &&
                           b.BillDate <= toDate &&
                           b.CustomerId.HasValue)
                    .GroupBy(b => new { b.CustomerId, b.Customer!.Name, b.Customer.Phone })
                    .Select(g => new
                    {
                        CustomerId = g.Key.CustomerId,
                        CustomerName = g.Key.Name,
                        CustomerPhone = g.Key.Phone,
                        TotalBills = g.Count(),
                        TotalAmount = g.Sum(b => b.TotalAmount),
                        TotalOutstanding = g.Sum(b => b.BalanceAmount),
                        LastPurchaseDate = g.Max(b => b.BillDate)
                    })
                    .OrderByDescending(c => c.TotalAmount)
                    .ToListAsync();

                return customerReport.Cast<object>().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating customer report");
                return new List<object>();
            }
        }

        public async Task<List<object>> GetSupplierReportAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var supplierReport = await _context.Purchases
                    .Include(p => p.Supplier)
                    .Where(p => p.PurchaseDate >= fromDate &&
                           p.PurchaseDate <= toDate)
                    .GroupBy(p => new { p.SupplierId, p.Supplier.Name, p.Supplier.Phone })
                    .Select(g => new
                    {
                        SupplierId = g.Key.SupplierId,
                        SupplierName = g.Key.Name,
                        SupplierPhone = g.Key.Phone,
                        TotalPurchases = g.Count(),
                        TotalAmount = g.Sum(p => p.TotalAmount),
                        TotalOutstanding = g.Sum(p => p.BalanceAmount),
                        LastPurchaseDate = g.Max(p => p.PurchaseDate)
                    })
                    .OrderByDescending(s => s.TotalAmount)
                    .ToListAsync();

                return supplierReport.Cast<object>().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating supplier report");
                return new List<object>();
            }
        }

        public async Task<Dictionary<string, object>> GetDashboardDataAsync()
        {
            try
            {
                var today = DateTime.Today;
                var thisMonth = new DateTime(today.Year, today.Month, 1);

                // Default values
                var defaultData = new Dictionary<string, object>
                {
                    {"TodaysSales", 0m},
                    {"TodaysBillCount", 0},
                    {"TodaysIncome", 0m},
                    {"TodaysExpense", 0m},
                    {"MonthlySales", 0m},
                    {"MonthlyBillCount", 0},
                    {"MonthlyIncome", 0m},
                    {"MonthlyExpense", 0m},
                    {"TotalCustomers", 0},
                    {"TotalProducts", 0},
                    {"TotalSuppliers", 0},
                    {"TotalStaff", 0},
                    {"LowStockCount", 0},
                    {"ExpiringProductsCount", 0}
                };

                // Today's data - with null checks
                var todaysBills = await _context.Bills
                    .Where(b => b.IsActive && b.BillDate.Date == today)
                    .ToListAsync() ?? new List<Bill>();

                var todaysCashEntries = await _context.CashEntries
                    .Where(c => c.IsActive && c.EntryDate.Date == today)
                    .ToListAsync() ?? new List<CashEntry>();

                // This month's data - with null checks
                var monthlyBills = await _context.Bills
                    .Where(b => b.IsActive && b.BillDate >= thisMonth)
                    .ToListAsync() ?? new List<Bill>();

                var monthlyCashEntries = await _context.CashEntries
                    .Where(c => c.IsActive && c.EntryDate >= thisMonth)
                    .ToListAsync() ?? new List<CashEntry>();

                // Counts - safely handle potential database issues
                var totalCustomers = 0;
                var totalProducts = 0;
                var totalSuppliers = 0;
                var totalStaff = 0;
                var lowStockCount = 0;
                var expiringCount = 0;

                try
                {
                    totalCustomers = await _context.Customers.CountAsync(c => c.IsActive);
                    totalProducts = await _context.Products.CountAsync(p => p.IsActive);
                    totalSuppliers = await _context.Suppliers.CountAsync(s => s.IsActive);
                    totalStaff = await _context.Staff.CountAsync(s => s.IsActive);
                    lowStockCount = await _context.Products.CountAsync(p => p.IsActive && p.CurrentStock <= p.MinimumStock);
                    expiringCount = await _context.Products
                        .CountAsync(p => p.IsActive && p.ExpiryDate.HasValue &&
                                   p.ExpiryDate.Value <= DateTime.Now.AddDays(30) && p.CurrentStock > 0);
                }
                catch (Exception dbEx)
                {
                    _logger.LogWarning(dbEx, "Error accessing counts data, using defaults");
                }

                // Update the data with actual values
                defaultData["TodaysSales"] = todaysBills.Any() ? todaysBills.Sum(b => b.TotalAmount) : 0m;
                defaultData["TodaysBillCount"] = todaysBills.Count;
                defaultData["TodaysIncome"] = todaysCashEntries.Where(c => c.EntryType == "Income").Sum(c => c.Amount);
                defaultData["TodaysExpense"] = todaysCashEntries.Where(c => c.EntryType == "Expense").Sum(c => c.Amount);
                defaultData["MonthlySales"] = monthlyBills.Any() ? monthlyBills.Sum(b => b.TotalAmount) : 0m;
                defaultData["MonthlyBillCount"] = monthlyBills.Count;
                defaultData["MonthlyIncome"] = monthlyCashEntries.Where(c => c.EntryType == "Income").Sum(c => c.Amount);
                defaultData["MonthlyExpense"] = monthlyCashEntries.Where(c => c.EntryType == "Expense").Sum(c => c.Amount);
                defaultData["TotalCustomers"] = totalCustomers;
                defaultData["TotalProducts"] = totalProducts;
                defaultData["TotalSuppliers"] = totalSuppliers;
                defaultData["TotalStaff"] = totalStaff;
                defaultData["LowStockCount"] = lowStockCount;
                defaultData["ExpiringProductsCount"] = expiringCount;

                return defaultData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating dashboard data");

                // Always return a valid dictionary with all required keys
                return new Dictionary<string, object>
                {
                    {"TodaysSales", 0m},
                    {"TodaysBillCount", 0},
                    {"TodaysIncome", 0m},
                    {"TodaysExpense", 0m},
                    {"MonthlySales", 0m},
                    {"MonthlyBillCount", 0},
                    {"MonthlyIncome", 0m},
                    {"MonthlyExpense", 0m},
                    {"TotalCustomers", 0},
                    {"TotalProducts", 0},
                    {"TotalSuppliers", 0},
                    {"TotalStaff", 0},
                    {"LowStockCount", 0},
                    {"ExpiringProductsCount", 0}
                };
            }
        }
    }

    public class WhatsAppService
    {
        private readonly ILogger<WhatsAppService> _logger;
        private readonly AppDbContext _context;

        public WhatsAppService(ILogger<WhatsAppService> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<bool> SendBillNotificationAsync(int billId, string phoneNumber)
        {
            try
            {
                var bill = await _context.Bills
                    .Include(b => b.Items)
                    .ThenInclude(i => i.Product)
                    .FirstOrDefaultAsync(b => b.Id == billId);

                if (bill == null)
                {
                    _logger.LogWarning("Bill not found for WhatsApp notification: {BillId}", billId);
                    return false;
                }

                var message = $"Thank you for your purchase!\n" +
                             $"Bill No: {bill.BillNo}\n" +
                             $"Date: {bill.BillDate:dd/MM/yyyy}\n" +
                             $"Total Amount: ₹{bill.TotalAmount:N2}\n" +
                             $"Payment Mode: {bill.PaymentMode}\n\n" +
                             $"Items:\n";

                foreach (var item in bill.Items.Take(5))
                {
                    message += $"• {item.ProductName} - Qty: {item.Quantity} - ₹{item.Total:N2}\n";
                }

                if (bill.Items.Count > 5)
                {
                    message += $"... and {bill.Items.Count - 5} more items\n";
                }

                message += "\nThank you for shopping with us!";

                // Here you would integrate with actual WhatsApp API
                // For now, just log the message
                _logger.LogInformation("WhatsApp message to {Phone}: {Message}", phoneNumber, message);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending WhatsApp bill notification for bill {BillId}", billId);
                return false;
            }
        }

        public async Task<bool> SendPaymentReminderAsync(int customerId)
        {
            try
            {
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.Id == customerId);

                if (customer == null || string.IsNullOrEmpty(customer.Phone))
                {
                    _logger.LogWarning("Customer not found or no phone number for payment reminder: {CustomerId}", customerId);
                    return false;
                }

                var message = $"Dear {customer.Name},\n\n" +
                             $"This is a friendly reminder that you have an outstanding balance of ₹{customer.OutstandingAmount:N2}.\n\n" +
                             $"Please make the payment at your earliest convenience.\n\n" +
                             $"Thank you!";

                // Here you would integrate with actual WhatsApp API
                _logger.LogInformation("WhatsApp payment reminder to {Phone}: {Message}", customer.Phone, message);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending WhatsApp payment reminder for customer {CustomerId}", customerId);
                return false;
            }
        }

        public async Task<bool> SendLowStockAlertAsync(List<Product> lowStockProducts)
        {
            try
            {
                if (!lowStockProducts.Any())
                    return true;

                var message = "🚨 LOW STOCK ALERT 🚨\n\n" +
                             $"The following {lowStockProducts.Count} product(s) are running low:\n\n";

                foreach (var product in lowStockProducts.Take(10))
                {
                    message += $"• {product.Name} - Stock: {product.CurrentStock} {product.Unit}\n";
                }

                if (lowStockProducts.Count > 10)
                {
                    message += $"... and {lowStockProducts.Count - 10} more products\n";
                }

                message += "\nPlease restock these items soon!";

                // Here you would integrate with actual WhatsApp API
                _logger.LogInformation("WhatsApp low stock alert: {Message}", message);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending WhatsApp low stock alert");
                return false;
            }
        }

        public async Task<bool> SendExpiryAlertAsync(List<Product> expiringProducts)
        {
            try
            {
                if (!expiringProducts.Any())
                    return true;

                var message = "⚠️ PRODUCT EXPIRY ALERT ⚠️\n\n" +
                             $"The following {expiringProducts.Count} product(s) are expiring soon:\n\n";

                foreach (var product in expiringProducts.Take(10))
                {
                    var daysLeft = product.ExpiryDate.HasValue
                        ? (product.ExpiryDate.Value - DateTime.Now).Days
                        : 0;
                    message += $"• {product.Name} - Expires in {daysLeft} day(s)\n";
                }

                if (expiringProducts.Count > 10)
                {
                    message += $"... and {expiringProducts.Count - 10} more products\n";
                }

                message += "\nPlease take necessary action!";

                // Here you would integrate with actual WhatsApp API
                _logger.LogInformation("WhatsApp expiry alert: {Message}", message);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending WhatsApp expiry alert");
                return false;
            }
        }
    }

    public class ThemeService
    {
        private string _currentTheme = "light";

        public string CurrentTheme => _currentTheme;

        public event Action<string>? ThemeChanged;

        public void SetTheme(string theme)
        {
            if (theme != _currentTheme)
            {
                _currentTheme = theme;
                ThemeChanged?.Invoke(_currentTheme);
            }
        }

        public bool IsDarkMode => _currentTheme == "dark";
    }
}
