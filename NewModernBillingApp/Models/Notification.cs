using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewModernBillingApp.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Message { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; } = "Info"; // Info, Success, Warning, Error, System

        [StringLength(100)]
        public string? Category { get; set; } // Stock, Payment, Expiry, Staff, Bill, etc.

        [StringLength(50)]
        public string Priority { get; set; } = "Normal"; // Low, Normal, High, Critical

        public int? UserId { get; set; } // Null = broadcast to all users
        public User? User { get; set; }

        public int? RoleId { get; set; } // Send to all users with this role
        public UserRole? Role { get; set; }

        public bool IsRead { get; set; } = false;

        public bool IsSystem { get; set; } = false; // System generated notifications

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? ReadDate { get; set; }

        public DateTime? ScheduledFor { get; set; } // For scheduled notifications

        public DateTime? ExpiryDate { get; set; }

        [StringLength(500)]
        public string? ActionUrl { get; set; } // URL to redirect when clicked

        [StringLength(100)]
        public string? ActionText { get; set; } // Text for action button

        [StringLength(200)]
        public string? Icon { get; set; } // CSS icon class

        [StringLength(50)]
        public string? Color { get; set; } // Badge/notification color

        public bool IsActive { get; set; } = true;

        public int? CreatedByUserId { get; set; }
        public User? CreatedBy { get; set; }

        [StringLength(500)]
        public string? Metadata { get; set; } // JSON data for additional info

        public bool IsDismissible { get; set; } = true;

        public bool ShowToast { get; set; } = false; // Show as toast notification

        public bool SendEmail { get; set; } = false;

        public bool SendWhatsApp { get; set; } = false;
    }

    public class NotificationTemplate
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string TemplateName { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string TitleTemplate { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string MessageTemplate { get; set; } = string.Empty;

        [StringLength(50)]
        public string Type { get; set; } = "Info";

        [StringLength(100)]
        public string Category { get; set; } = string.Empty;

        [StringLength(50)]
        public string Priority { get; set; } = "Normal";

        [StringLength(200)]
        public string? Icon { get; set; }

        [StringLength(50)]
        public string? Color { get; set; }

        public bool IsActive { get; set; } = true;

        public bool ShowToast { get; set; } = false;

        public bool SendEmail { get; set; } = false;

        public bool SendWhatsApp { get; set; } = false;

        [StringLength(300)]
        public string? Description { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [StringLength(1000)]
        public string? PlaceholderDescription { get; set; } // Description of available placeholders
    }

    public class AppSettings
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string SettingKey { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string SettingValue { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Category { get; set; }

        [StringLength(200)]
        public string? DisplayName { get; set; }

        [StringLength(300)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string DataType { get; set; } = "String"; // String, Number, Boolean, JSON

        public bool IsSystemSetting { get; set; } = false;

        public bool IsUserEditable { get; set; } = true;

        public bool RequiresRestart { get; set; } = false;

        [StringLength(1000)]
        public string? ValidationRules { get; set; } // JSON validation rules

        [StringLength(1000)]
        public string? PossibleValues { get; set; } // For dropdown/radio options

        public int? SortOrder { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? LastUpdated { get; set; }

        public int? UpdatedByUserId { get; set; }
        public User? UpdatedBy { get; set; }
    }

    public class UserPreference
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string PreferenceKey { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string PreferenceValue { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Category { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? LastUpdated { get; set; }
    }

    public class AuditLog
    {
        public int Id { get; set; }

        public int? UserId { get; set; }
        public User? User { get; set; }

        [Required]
        [StringLength(100)]
        public string Action { get; set; } = string.Empty; // Create, Update, Delete, Login, etc.

        [Required]
        [StringLength(100)]
        public string EntityType { get; set; } = string.Empty; // Product, Customer, Bill, etc.

        public int? EntityId { get; set; }

        [StringLength(200)]
        public string? EntityName { get; set; }

        [StringLength(1000)]
        public string? OldValues { get; set; } // JSON

        [StringLength(1000)]
        public string? NewValues { get; set; } // JSON

        [StringLength(45)]
        public string? IPAddress { get; set; }

        [StringLength(500)]
        public string? UserAgent { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        [StringLength(300)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string? Result { get; set; } // Success, Failed, Error

        [StringLength(500)]
        public string? ErrorMessage { get; set; }
    }

    public class CompanyInfo
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        [StringLength(300)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        [StringLength(50)]
        public string? State { get; set; }

        [StringLength(10)]
        public string? PinCode { get; set; }

        [StringLength(50)]
        public string? Country { get; set; } = "India";

        [StringLength(15)]
        public string? Phone { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(200)]
        public string? Website { get; set; }

        [StringLength(15)]
        public string? GSTNumber { get; set; }

        [StringLength(50)]
        public string? PANNumber { get; set; }

        [StringLength(100)]
        public string? BankName { get; set; }

        [StringLength(50)]
        public string? BankAccountNumber { get; set; }

        [StringLength(20)]
        public string? IFSCCode { get; set; }

        [StringLength(500)]
        public string? LogoPath { get; set; }

        [StringLength(500)]
        public string? SignaturePath { get; set; }

        [StringLength(1000)]
        public string? TermsAndConditions { get; set; }

        [StringLength(500)]
        public string? InvoiceFooter { get; set; }

        [StringLength(20)]
        public string? CurrencySymbol { get; set; } = "₹";

        [StringLength(10)]
        public string? CurrencyCode { get; set; } = "INR";

        [StringLength(50)]
        public string? FinancialYearStart { get; set; } = "April";

        [StringLength(20)]
        public string? TaxationType { get; set; } = "GST"; // GST, VAT, None

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? LastUpdated { get; set; }

        public int? UpdatedByUserId { get; set; }
        public User? UpdatedBy { get; set; }
    }
}
