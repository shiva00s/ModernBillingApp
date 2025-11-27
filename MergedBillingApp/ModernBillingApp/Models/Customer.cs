using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ModernBillingApp.Models;

// This C# class replaces your "Customer_Entry" SQL table
public class Customer
{
    public int Id { get; set; } // Replaces AutoID
    public string? CID { get; set; } // Your old Customer ID

    [Required(ErrorMessage = "Customer name is required.")]
    public string? CName { get; set; }
    public string? CAddress { get; set; }
    public string? Gender { get; set; }
    public string? CContact { get; set; }
    public string? CCity { get; set; }
    public string? CState { get; set; }
    public string? CGST { get; set; }
    public string? CEmail { get; set; }
    public string? CPincode { get; set; }
    public DateTime? CDate { get; set; }
    public string? CardNo { get; set; }
    
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Points { get; set; } = 0; // Current loyalty points balance
    
    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalPointsEarned { get; set; } = 0; // Lifetime points earned
    
    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalPointsRedeemed { get; set; } = 0; // Lifetime points redeemed
    
    public string? CusType { get; set; }
    
    [Column(TypeName = "decimal(18, 2)")]
    public decimal CreditLimit { get; set; } = 0; // Maximum credit allowed

    // Customer balance tracking
    [Column(TypeName = "decimal(18, 2)")]
    public decimal OutstandingBalance { get; set; } = 0; // Total outstanding amount

    // This links the Customer to their list of payments
    public ICollection<CustomerPayment> Payments { get; set; } = new List<CustomerPayment>();
    
    // Enhanced payments with partial payment support
    public ICollection<CustomerPaymentEnhanced> EnhancedPayments { get; set; } = new List<CustomerPaymentEnhanced>();
    
    // Loyalty transactions
    public ICollection<LoyaltyTransaction> LoyaltyTransactions { get; set; } = new List<LoyaltyTransaction>();
    
    // Bills (for tracking)
    public ICollection<Bill> Bills { get; set; } = new List<Bill>();
}

// This C# class REPLACES your "Customer_Payment" SQL table
// It's much simpler and stores the main payment details.
public class CustomerPayment
{
    public int Id { get; set; } // Replaces AutoID
    public DateTime PaymentDate { get; set; }
    public string PaymentMode { get; set; } // "Cash", "Cheque", "Card", "NEFT"

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    public string? BillNo { get; set; }
    
    public int? BillId { get; set; } // Link to specific bill
    public Bill? Bill { get; set; }
    
    [StringLength(100)]
    public string? ReferenceNo { get; set; } // Cheque No, UPI Ref, etc.
    
    [StringLength(500)]
    public string? Remarks { get; set; }

    // This links the payment back to the Customer
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }
    
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public string? CreatedBy { get; set; }
}