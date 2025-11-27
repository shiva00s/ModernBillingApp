using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModernBillingApp.Models
{
    // Supplier Payment Model
    public class SupplierPayment
    {
        public int Id { get; set; }
        public DateTime PaymentDate { get; set; }
        
        [Required]
        [StringLength(50)]
        public string PaymentMode { get; set; } = "Cash"; // Cash, Cheque, NEFT, UPI, Card
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }
        
        [StringLength(100)]
        public string? ReferenceNo { get; set; } // Cheque No, UPI Ref, etc.
        
        [StringLength(50)]
        public string? PurchaseNo { get; set; } // Link to purchase entry
        
        [StringLength(500)]
        public string? Remarks { get; set; }
        
        // Link to Supplier
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        
        // Link to Purchase Entry (if bill-wise payment)
        public int? PurchaseEntryId { get; set; }
        public PurchaseEntry? PurchaseEntry { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
    }

    // Enhanced Customer Payment with Partial Payment Support
    public class CustomerPaymentEnhanced
    {
        public int Id { get; set; }
        public DateTime PaymentDate { get; set; }
        
        [Required]
        [StringLength(50)]
        public string PaymentMode { get; set; } = "Cash";
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }
        
        // Bill-wise payment tracking
        [StringLength(50)]
        public string? BillNo { get; set; }
        public int? BillId { get; set; }
        public Bill? Bill { get; set; }
        
        // Partial payment tracking
        [Column(TypeName = "decimal(18, 2)")]
        public decimal BillAmount { get; set; } // Original bill amount
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PreviousPaid { get; set; } // Previously paid
        [Column(TypeName = "decimal(18, 2)")]
        public decimal CurrentPayment { get; set; } // This payment
        [Column(TypeName = "decimal(18, 2)")]
        public decimal RemainingBalance { get; set; } // Remaining after this payment
        public bool IsFullPayment { get; set; } = false;
        
        [StringLength(100)]
        public string? ReferenceNo { get; set; }
        
        [StringLength(500)]
        public string? Remarks { get; set; }
        
        // Link to Customer
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
    }

    // Purchase Entry Model
    public class PurchaseEntry
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string PurchaseNo { get; set; } // Purchase Invoice No
        
        public DateTime PurchaseDate { get; set; }
        
        // Supplier Information
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        
        [StringLength(200)]
        public string? SupplierName { get; set; } // Snapshot at time of purchase
        [StringLength(50)]
        public string? SupplierGSTIN { get; set; }
        
        // Purchase Totals
        [Column(TypeName = "decimal(18, 2)")]
        public decimal SubTotal { get; set; }
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal CGSTAmount { get; set; } = 0;
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal SGSTAmount { get; set; } = 0;
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal IGSTAmount { get; set; } = 0;
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalGST { get; set; }
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal GrandTotal { get; set; }
        
        // Payment Information
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PaidAmount { get; set; } = 0;
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal BalanceAmount { get; set; } = 0;
        
        [StringLength(50)]
        public string? PaymentStatus { get; set; } = "Pending"; // Pending, Partial, Paid
        
        [StringLength(500)]
        public string? Remarks { get; set; }
        
        // Purchase Items
        public ICollection<PurchaseItem> Items { get; set; } = new List<PurchaseItem>();
        
        // Payments made against this purchase
        public ICollection<SupplierPayment> Payments { get; set; } = new List<SupplierPayment>();
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
    }

    // Purchase Item
    public class PurchaseItem
    {
        public int Id { get; set; }
        
        public int PurchaseEntryId { get; set; }
        public PurchaseEntry PurchaseEntry { get; set; }
        
        public int ProductId { get; set; }
        public Product Product { get; set; }
        
        [StringLength(50)]
        public string? ProductCode { get; set; }
        [StringLength(200)]
        public string? ProductName { get; set; }
        [StringLength(10)]
        public string? HSNCode { get; set; }
        
        public double Quantity { get; set; }
        
        [StringLength(20)]
        public string? Unit { get; set; } = "PCS";
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PurchasePrice { get; set; }
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal GSTRate { get; set; }
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal GSTAmount { get; set; }
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Total { get; set; }
        
        [StringLength(50)]
        public string? BatchNo { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }

    // Purchase Return Model
    public class PurchaseReturn
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string ReturnNo { get; set; }
        
        public DateTime ReturnDate { get; set; }
        
        public int PurchaseEntryId { get; set; }
        public PurchaseEntry PurchaseEntry { get; set; }
        
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }
        
        [StringLength(500)]
        public string? Reason { get; set; }
        
        [StringLength(500)]
        public string? Remarks { get; set; }
        
        public ICollection<PurchaseReturnItem> Items { get; set; } = new List<PurchaseReturnItem>();
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
    }

    // Purchase Return Item
    public class PurchaseReturnItem
    {
        public int Id { get; set; }
        
        public int PurchaseReturnId { get; set; }
        public PurchaseReturn PurchaseReturn { get; set; }
        
        public int PurchaseItemId { get; set; }
        public PurchaseItem PurchaseItem { get; set; }
        
        public int ProductId { get; set; }
        public Product Product { get; set; }
        
        public double ReturnQuantity { get; set; }
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal ReturnAmount { get; set; }
        
        [StringLength(500)]
        public string? Reason { get; set; }
    }

    // Bill Return Model
    // Loyalty Points Transaction
    public class LoyaltyTransaction
    {
        public int Id { get; set; }
        
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        
        public DateTime TransactionDate { get; set; }
        
        [StringLength(50)]
        public string TransactionType { get; set; } // Earn, Redeem, Expire, Adjust
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Points { get; set; } // Positive for earn, negative for redeem
        
        [StringLength(50)]
        public string? BillNo { get; set; }
        public int? BillId { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal BalanceAfter { get; set; } // Points balance after this transaction
        
        public DateTime? ExpiryDate { get; set; } // When points expire
    }

    // Expiry Alert Model
    public class ExpiryAlert
    {
        public int Id { get; set; }
        
        public int ProductId { get; set; }
        // Note: Product navigation property removed to avoid type conflict
        // Use ProductId to query Product separately if needed
        
        [StringLength(50)]
        public string? BatchNo { get; set; }
        
        public DateTime ExpiryDate { get; set; }
        
        public double Quantity { get; set; }
        
        public int DaysUntilExpiry { get; set; }
        
        [StringLength(50)]
        public string AlertLevel { get; set; } = "Normal"; // Normal, Warning, Critical, Expired
        
        public bool IsNotified { get; set; } = false;
        public DateTime? NotifiedDate { get; set; }
    }
}

