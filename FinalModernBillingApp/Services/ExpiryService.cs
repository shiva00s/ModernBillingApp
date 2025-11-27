using Microsoft.EntityFrameworkCore;
using ModernBillingApp.Data;
using ModernBillingApp.Models;

namespace ModernBillingApp.Services
{
    // Expiry Date Management Service
    public class ExpiryService
    {
        private readonly AppDbContext _context;

        public ExpiryService(AppDbContext context)
        {
            _context = context;
        }

        // Get products expiring soon
        public async Task<List<ExpiryAlert>> GetExpiringProducts(int daysAhead = 30)
        {
            var expiryDate = DateTime.Now.AddDays(daysAhead);

            var products = await _context.Products
                .Where(p => p.ExpiryDate.HasValue && 
                           p.ExpiryDate.Value <= expiryDate &&
                           p.CurrentStock > 0)
                .ToListAsync();

            var alerts = new List<ExpiryAlert>();

            foreach (var product in products)
            {
                if (!product.ExpiryDate.HasValue) continue;

                var daysUntilExpiry = (product.ExpiryDate.Value - DateTime.Now).Days;
                string alertLevel = daysUntilExpiry switch
                {
                    < 0 => "Expired",
                    <= 7 => "Critical",
                    <= 15 => "Warning",
                    _ => "Normal"
                };

                alerts.Add(new ExpiryAlert
                {
                    ProductId = product.Id,
                    BatchNo = product.BatchNo,
                    ExpiryDate = product.ExpiryDate.Value,
                    Quantity = (double)product.CurrentStock,
                    DaysUntilExpiry = daysUntilExpiry,
                    AlertLevel = alertLevel
                });
            }

            return alerts.OrderBy(a => a.ExpiryDate).ToList();
        }

        // Get expired products
        public async Task<List<Product>> GetExpiredProducts()
        {
            return await _context.Products
                .Where(p => p.ExpiryDate.HasValue && 
                           p.ExpiryDate.Value < DateTime.Now &&
                           p.CurrentStock > 0)
                .OrderBy(p => p.ExpiryDate)
                .ToListAsync();
        }

        // Check and create expiry alerts
        public async Task<List<ExpiryAlert>> CheckExpiryAlerts()
        {
            var alerts = await GetExpiringProducts(30);
            
            // Save alerts to database
            foreach (var alert in alerts)
            {
                var existing = await _context.ExpiryAlerts
                    .FirstOrDefaultAsync(a => a.ProductId == alert.ProductId && 
                                             a.BatchNo == alert.BatchNo);
                
                if (existing == null)
                {
                    _context.ExpiryAlerts.Add(alert);
                }
                else
                {
                    existing.DaysUntilExpiry = alert.DaysUntilExpiry;
                    existing.AlertLevel = alert.AlertLevel;
                    existing.Quantity = alert.Quantity;
                }
            }

            await _context.SaveChangesAsync();
            return alerts;
        }

        // Get expiry summary for dashboard
        public async Task<ExpirySummary> GetExpirySummary()
        {
            var today = DateTime.Now;
            var weekFromNow = today.AddDays(7);
            var monthFromNow = today.AddDays(30);

            var expired = await _context.Products
                .Where(p => p.ExpiryDate.HasValue && 
                           p.ExpiryDate.Value < today &&
                           p.CurrentStock > 0)
                .CountAsync();

            var expiringThisWeek = await _context.Products
                .Where(p => p.ExpiryDate.HasValue && 
                           p.ExpiryDate.Value >= today &&
                           p.ExpiryDate.Value <= weekFromNow &&
                           p.CurrentStock > 0)
                .CountAsync();

            var expiringThisMonth = await _context.Products
                .Where(p => p.ExpiryDate.HasValue && 
                           p.ExpiryDate.Value >= today &&
                           p.ExpiryDate.Value <= monthFromNow &&
                           p.CurrentStock > 0)
                .CountAsync();

            return new ExpirySummary
            {
                ExpiredCount = expired,
                ExpiringThisWeek = expiringThisWeek,
                ExpiringThisMonth = expiringThisMonth
            };
        }
    }

    public class ExpirySummary
    {
        public int ExpiredCount { get; set; }
        public int ExpiringThisWeek { get; set; }
        public int ExpiringThisMonth { get; set; }
    }
}

