using Microsoft.EntityFrameworkCore;
using ModernBillingApp.Data;
using ModernBillingApp.Models;

namespace ModernBillingApp.Services
{
    // Notification Service
    public class NotificationService
    {
        private readonly AppDbContext _context;

        public NotificationService(AppDbContext context)
        {
            _context = context;
        }

        // Create notification
        public async Task<Notification> CreateNotification(
            string title,
            string? message = null,
            string type = "Info",
            string? category = null,
            int? userId = null,
            string? actionUrl = null)
        {
            var notification = new Notification
            {
                Title = title,
                Message = message,
                Type = type,
                Category = category,
                UserId = userId,
                ActionUrl = actionUrl,
                CreatedDate = DateTime.Now
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        // Get unread notifications for user
        public async Task<List<Notification>> GetUnreadNotifications(int? userId = null)
        {
            var query = _context.Notifications
                .Where(n => !n.IsRead);

            if (userId.HasValue)
            {
                query = query.Where(n => n.UserId == null || n.UserId == userId.Value);
            }
            else
            {
                query = query.Where(n => n.UserId == null); // Broadcast only
            }

            return await query
                .OrderByDescending(n => n.CreatedDate)
                .Take(50)
                .ToListAsync();
        }

        // Mark notification as read
        public async Task MarkAsRead(int notificationId, int? userId = null)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                notification.ReadDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        // Mark all as read for user
        public async Task MarkAllAsRead(int? userId = null)
        {
            var query = _context.Notifications.Where(n => !n.IsRead);
            
            if (userId.HasValue)
            {
                query = query.Where(n => n.UserId == null || n.UserId == userId.Value);
            }
            else
            {
                query = query.Where(n => n.UserId == null);
            }

            var notifications = await query.ToListAsync();
            foreach (var n in notifications)
            {
                n.IsRead = true;
                n.ReadDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }

        // Get notification count
        public async Task<int> GetUnreadCount(int? userId = null)
        {
            var query = _context.Notifications.Where(n => !n.IsRead);
            
            if (userId.HasValue)
            {
                query = query.Where(n => n.UserId == null || n.UserId == userId.Value);
            }
            else
            {
                query = query.Where(n => n.UserId == null);
            }

            return await query.CountAsync();
        }

        // Create stock alert notification
        public async Task CreateStockAlert(int productId, string productName, double currentStock)
        {
            await CreateNotification(
                title: $"Low Stock Alert: {productName}",
                message: $"Only {currentStock} units remaining. Please restock soon.",
                type: "Warning",
                category: "Stock",
                actionUrl: $"/product-catalog"
            );
        }

        // Create expiry alert notification
        public async Task CreateExpiryAlert(int productId, string productName, DateTime expiryDate, int daysUntilExpiry)
        {
            await CreateNotification(
                title: $"Expiry Alert: {productName}",
                message: $"Product expires in {daysUntilExpiry} days ({expiryDate:dd MMM yyyy})",
                type: daysUntilExpiry <= 7 ? "Error" : "Warning",
                category: "Expiry",
                actionUrl: $"/product-catalog"
            );
        }
    }
}

