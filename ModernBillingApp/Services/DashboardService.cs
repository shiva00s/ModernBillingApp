using Microsoft.EntityFrameworkCore;
using ModernBillingApp.Data;
using ModernBillingApp.Models;

// This service provides all the numbers for our new dashboard.
public class DashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    // This will hold all our dashboard numbers
    public class DashboardStats
    {
        public decimal TodaySales { get; set; }
        public decimal TodayExpenses { get; set; }
        public int NewCustomersToday { get; set; }
        public int LowStockItems { get; set; }
    }

    // This function gets all the stats at once
    public async Task<DashboardStats> GetDashboardStats()
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        // 1. Get Today's Sales
        var todaySales = await _context.Bills
            .Where(b => b.BDate >= today && b.BDate < tomorrow)
            .SumAsync(b => b.GrandTotal);

        // 2. Get Today's Expenses
        var todayExpenses = await _context.CashEntries
            .Where(c => c.CEType == "Expense" && c.Date >= today && c.Date < tomorrow)
            .SumAsync(c => c.Amount);

        // 3. Get New Customers Today
        var newCustomers = await _context.Customers
            .Where(c => c.CDate >= today && c.CDate < tomorrow)
            .CountAsync();

        // 4. Get Low Stock Items (Improvement: We'll set 10 as the limit)
        var lowStock = await _context.Products
            .Where(p => p.CurrentStock <= 10)
            .CountAsync();

        return new DashboardStats
        {
            TodaySales = todaySales,
            TodayExpenses = todayExpenses,
            NewCustomersToday = newCustomers,
            LowStockItems = lowStock
        };
    }
}