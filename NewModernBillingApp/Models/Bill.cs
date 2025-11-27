using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewModernBillingApp.Models
{
    public class Bill
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string BillNo { get; set; } = string.Empty;

        public DateTime BillDate { get; set; } = DateTime.Now;

        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }

        [StringLength(100)]
        public string? CustomerName { get; set; }

        [StringLength(15)]
        public string? CustomerPhone { get; set; }

        [StringLength(300)]
        public string? CustomerAddress { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal SubTotal { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal DiscountAmount { get; set; } = 0;

        [Column(TypeName = "decimal(8, 2)")]
        public decimal DiscountPercentage { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal GSTAmount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal CGSTAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal SGSTAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal IGSTAmount { get; set; } = 0;

        public bool IsInterState { get; set; } = false;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PaidAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal BalanceAmount { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMode { get; set; } = "Cash"; // Cash, Card, UPI, NEFT, Credit

        [StringLength(100)]
        public string? PaymentReference { get; set; }

        [StringLength(50)]
        public string PaymentStatus { get; set; } = "Paid"; // Paid, Partial, Pending

        [StringLength(100)]
        public string? SalesPerson { get; set; }

        [StringLength(200)]
        public string? Notes { get; set; }

        public bool IsPrinted { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public int CreatedByUserId { get; set; }
        public User CreatedBy { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? LastUpdated { get; set; }

        // Navigation properties
        public ICollection<BillItem> Items { get; set; } = new List<BillItem>();
        public ICollection<CustomerPayment> Payments { get; set; } = new List<CustomerPayment>();
    }

    public class BillItem
    {
        public int Id { get; set; }

        public int BillId { get; set; }
        public Bill Bill { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        [StringLength(100)]
        public string ProductName { get; set; } = string.Empty;

        [StringLength(50)]
        public string? ProductCode { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Quantity { get; set; }

        [StringLength(20)]
        public string Unit { get; set; } = "PCS";

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Rate { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal MRP { get; set; }

        [Column(TypeName = "decimal(8, 2)")]
        public decimal DiscountPercentage { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal DiscountAmount { get; set; } = 0;

        [Column(TypeName = "decimal(8, 2)")]
        public decimal GST { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal GSTAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal CGSTAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal SGSTAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal IGSTAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Total { get; set; }

        [StringLength(20)]
        public string? HSNCode { get; set; }

        [StringLength(50)]
        public string? BatchNo { get; set; }

        public DateTime? ExpiryDate { get; set; }
    }

    public class CashEntry
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string EntryName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string EntryType { get; set; } = string.Empty; // Income, Expense

        [StringLength(100)]
        public string? Category { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        public DateTime EntryDate { get; set; } = DateTime.Now;

        [StringLength(300)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? PaymentMode { get; set; } = "Cash";

        [StringLength(100)]
        public string? TransactionReference { get; set; }

        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public int? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        [StringLength(100)]
        public string? ReceivedFrom { get; set; }

        [StringLength(100)]
        public string? PaidTo { get; set; }

        public bool IsActive { get; set; } = true;

        public int CreatedByUserId { get; set; }
        public User CreatedBy { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? LastUpdated { get; set; }

        [StringLength(200)]
        public string? Notes { get; set; }
    }

    public class BillReturn
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ReturnNo { get; set; } = string.Empty;

        public DateTime ReturnDate { get; set; } = DateTime.Now;

        public int OriginalBillId { get; set; }
        public Bill OriginalBill { get; set; } = null!;

        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }

        [StringLength(100)]
        public string? Reason { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal SubTotal { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal GSTAmount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        [StringLength(50)]
        public string RefundMode { get; set; } = "Cash";

        [StringLength(100)]
        public string? RefundReference { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Processed"; // Processed, Refunded

        public bool IsActive { get; set; } = true;

        public int CreatedByUserId { get; set; }
        public User CreatedBy { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [StringLength(300)]
        public string? Notes { get; set; }

        // Navigation properties
        public ICollection<BillReturnItem> Items { get; set; } = new List<BillReturnItem>();
    }

    public class BillReturnItem
    {
        public int Id { get; set; }

        public int BillReturnId { get; set; }
        public BillReturn BillReturn { get; set; } = null!;

        public int OriginalBillItemId { get; set; }
        public BillItem OriginalBillItem { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        [Column(TypeName = "decimal(10, 2)")]
        public decimal ReturnQuantity { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Rate { get; set; }

        [Column(TypeName = "decimal(8, 2)")]
        public decimal GST { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal GSTAmount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Total { get; set; }

        [StringLength(100)]
        public string? Reason { get; set; }
    }
}
