using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModernBillingApp.Models
{
    // Notification Model
    public class Notification
    {
        public int Id { get; set; }
        
        [StringLength(200)]
        public string Title { get; set; } = "";
        
        [StringLength(1000)]
        public string? Message { get; set; }
        
        [StringLength(50)]
        public string Type { get; set; } = "Info"; // Info, Success, Warning, Error
        
        [StringLength(50)]
        public string? Category { get; set; } // Stock, Payment, Expiry, etc.
        
        public int? UserId { get; set; } // Null = broadcast to all users
        public User? User { get; set; }
        
        public bool IsRead { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ReadDate { get; set; }
        
        [StringLength(200)]
        public string? ActionUrl { get; set; } // URL to redirect when clicked
    }
}

