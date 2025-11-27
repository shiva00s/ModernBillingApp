using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ModernBillingApp.Models;

// This C# class replaces your "Customer_Entry" SQL table and merges with NewModernBillingApp's Customer model
public class Customer
{
    public int Id { get; set; } // Replaces AutoID

    public string? CID { get; set; } // Your old Customer ID (from ModernBillingApp)

    [Required(ErrorMessage = "Customer name is required.")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty; // Combined CName and Name

    [StringLength(300)]
    public string? Address { get; set; } // Combined CAddress and Address

    public string? Gender { get; set; } // From ModernBillingApp

    [StringLength(15)]
    public string? Phone { get; set; } // Combined CContact and Phone

    [StringLength(50)]
    public string? City { get; set; } // Combined CCity and City

    [StringLength(50)]
    public string? State { get; set; } // Combined CState and State

    [StringLength(15)]
    public string? GSTNumber { get; set; } // Combined CGST and GSTNumber

    [EmailAddress]
    [StringLength(100)]
    public string? Email { get; set; } // Combined CEmail and Email

    [StringLength(10)]
    public string? PinCode { get; set; } // Combined CPincode and PinCode

    public DateTime CreatedDate { get; set; } = DateTime.Now; // Renamed CDate to CreatedDate, with default from NewModernBillingApp
    public DateTime? LastUpdated { get; set; } // From NewModernBillingApp

    public string? CardNo { get; set; } // From ModernBillingApp

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Points { get; set; } = 0; // Current loyalty points balance (from ModernBillingApp)

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalPointsEarned { get; set; } = 0; // Lifetime points earned (from ModernBillingApp)

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalPointsRedeemed { get; set; } = 0; // Lifetime points redeemed (from ModernBillingApp)

    [StringLength(50)]
    public string? CustomerType { get; set; } = "Regular"; // Combined CusType and CustomerType, with default from NewModernBillingApp

    [Column(TypeName = "decimal(18, 2)")]
    public decimal CreditLimit { get; set; } = 0; // Maximum credit allowed (merged default)

    // Customer balance tracking
    [Column(TypeName = "decimal(18, 2)")]
    public decimal OutstandingBalance { get; set; } = 0; // Total outstanding amount (merged OutstandingBalance and OutstandingAmount)

    public bool IsActive { get; set; } = true; // From NewModernBillingApp

    [StringLength(200)]
    public string? Notes { get; set; } // From NewModernBillingApp

    // This links the Customer to their list of payments
    public ICollection<CustomerPayment> Payments { get; set; } = new List<CustomerPayment>();

    // Enhanced payments with partial payment support (from ModernBillingApp)
    public ICollection<CustomerPaymentEnhanced> EnhancedPayments { get; set; } = new List<CustomerPaymentEnhanced>();

    // Loyalty transactions (from ModernBillingApp)
    public ICollection<LoyaltyTransaction> LoyaltyTransactions { get; set; } = new List<LoyaltyTransaction>();

    // Bills (for tracking)
    public ICollection<Bill> Bills { get; set; } = new List<Bill>();

    // Backward compatibility properties (NotMapped)
    [NotMapped]
    public DateTime CDate
    {
        get => CreatedDate;
        set => CreatedDate = value;
    }

    [NotMapped]
    public string? CName
    {
        get => Name;
        set => Name = value ?? string.Empty;
    }

    [NotMapped]
    public string? CContact
    {
        get => Phone;
        set => Phone = value;
    }

    [NotMapped]
    public string? CAddress
    {
        get => Address;
        set => Address = value;
    }

    [NotMapped]
    public string? CCity
    {
        get => City;
        set => City = value;
    }

    [NotMapped]
    public string? CState
    {
        get => State;
        set => State = value;
    }

    [NotMapped]
    public string? CGST
    {
        get => GSTNumber;
        set => GSTNumber = value;
    }

    [NotMapped]
    public string? CEmail
    {
        get => Email;
        set => Email = value;
    }

    [NotMapped]
    public string? CPincode
    {
        get => PinCode;
        set => PinCode = value;
    }
}

// This C# class REPLACES your "Customer_Payment" SQL table and merges with NewModernBillingApp's CustomerPayment model
public class CustomerPayment
{
    public int Id { get; set; } // Replaces AutoID
    public DateTime PaymentDate { get; set; } = DateTime.Now; // With default from NewModernBillingApp

    [Required]
    [StringLength(50)]
    public string PaymentMode { get; set; } = string.Empty; // "Cash", "Cheque", "Card", "NEFT", "UPI", "Bank Transfer"

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    public int? BillId { get; set; } // Link to specific bill (from ModernBillingApp)
    public Bill? Bill { get; set; }

    [StringLength(100)]
    public string? TransactionReference { get; set; } // Combined ReferenceNo and TransactionReference

    [StringLength(300)]
    public string? Notes { get; set; } // Combined Remarks and Notes

    // This links the payment back to the Customer
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!; // Added null! from NewModernBillingApp

    public int CreatedByUserId { get; set; } // From NewModernBillingApp
    public User CreatedBy { get; set; } = null!; // From NewModernBillingApp
}
