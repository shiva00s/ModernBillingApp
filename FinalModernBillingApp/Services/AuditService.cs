using Microsoft.EntityFrameworkCore;
using ModernBillingApp.Data;
using System;

namespace ModernBillingApp.Services
{
    // Audit trail for tracking all important operations
    public class AuditLog
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string? UserName { get; set; }
        public string? Action { get; set; } // CREATE, UPDATE, DELETE, VIEW
        public string? EntityType { get; set; } // Bill, Product, Customer, etc.
        public int? EntityId { get; set; }
        public string? Details { get; set; }
        public string? IPAddress { get; set; }
    }

    public class AuditService
    {
        private readonly AppDbContext _context;

        public AuditService(AppDbContext context)
        {
            _context = context;
        }

        public async Task LogAction(string userName, string action, string entityType, int? entityId, string? details = null)
        {
            var log = new AuditLog
            {
                Timestamp = DateTime.Now,
                UserName = userName,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                Details = details
            };

            // Note: You'll need to add AuditLogs to DbContext
            // _context.AuditLogs.Add(log);
            // await _context.SaveChangesAsync();
        }
    }
}

