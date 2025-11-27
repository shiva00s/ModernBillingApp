using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewModernBillingApp.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string? ProductCode { get; set; }

        [StringLength(20)]
        public string? Barcode { get; set; }

        [StringLength(100)]
        public string? Category { get; set; }

        [StringLength(50)]
        public string? Brand { get; set; }

        [StringLength(20)]
        public string Unit { get; set; } = "PCS"; // PCS, KG, LTR, MTR

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PurchasePrice { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal SellingPrice { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal MRP { get; set; }

        [Column(TypeName = "decimal(8, 2)")]
        public decimal GST { get; set; } = 0; // GST percentage

        [Column(TypeName = "decimal(10, 2)")]
        public decimal CurrentStock { get; set; } = 0;

        [Column(TypeName = "decimal(10, 2)")]
        public decimal MinimumStock { get; set; } = 0;

        [Column(TypeName = "decimal(10, 2)")]
        public decimal MaximumStock { get; set; } = 0;

        [StringLength(20)]
        public string? HSNCode { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? LastUpdated { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [StringLength(300)]
        public string? Description { get; set; }

        [StringLength(200)]
        public string? ImageUrl { get; set; }

        public int? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        // Navigation properties
        public ICollection<BillItem> BillItems { get; set; } = new List<BillItem>();
        public ICollection<StockLedger> StockLedgers { get; set; } = new List<StockLedger>();
        public ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
    }

    public class Supplier
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        public string? ContactPerson { get; set; }

        [StringLength(15)]
        public string? Phone { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(300)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        [StringLength(50)]
        public string? State { get; set; }

        [StringLength(10)]
        public string? PinCode { get; set; }

        [StringLength(15)]
        public string? GSTNumber { get; set; }

        [StringLength(50)]
        public string? PANNumber { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal CreditLimit { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal OutstandingAmount { get; set; } = 0;

        public int PaymentTerms { get; set; } = 30; // Days

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? LastUpdated { get; set; }

        [StringLength(200)]
        public string? Notes { get; set; }

        // Navigation properties
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
        public ICollection<SupplierPayment> Payments { get; set; } = new List<SupplierPayment>();
    }

    public class StockLedger
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string TransactionType { get; set; } = string.Empty; // Purchase, Sale, Return, Adjustment

        [StringLength(50)]
        public string? ReferenceNo { get; set; }

        public int? ReferenceId { get; set; } // BillId, PurchaseId, etc.

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Quantity { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Rate { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal StockBefore { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal StockAfter { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [StringLength(200)]
        public string? Notes { get; set; }

        public int CreatedByUserId { get; set; }
        public User CreatedBy { get; set; } = null!;
    }

    public class Purchase
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string PurchaseNo { get; set; } = string.Empty;

        public DateTime PurchaseDate { get; set; } = DateTime.Now;

        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; } = null!;

        [StringLength(50)]
        public string? SupplierInvoiceNo { get; set; }

        public DateTime? SupplierInvoiceDate { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal SubTotal { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal GSTAmount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PaidAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal BalanceAmount { get; set; }

        [StringLength(50)]
        public string PaymentStatus { get; set; } = "Pending"; // Pending, Partial, Paid

        [StringLength(200)]
        public string? Notes { get; set; }

        public int CreatedByUserId { get; set; }
        public User CreatedBy { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public ICollection<PurchaseItem> Items { get; set; } = new List<PurchaseItem>();
        public ICollection<SupplierPayment> Payments { get; set; } = new List<SupplierPayment>();
    }

    public class PurchaseItem
    {
        public int Id { get; set; }

        public int PurchaseId { get; set; }
        public Purchase Purchase { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Quantity { get; set; }

        [StringLength(20)]
        public string Unit { get; set; } = "PCS";

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Rate { get; set; }

        [Column(TypeName = "decimal(8, 2)")]
        public decimal GST { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal GSTAmount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Total { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [StringLength(50)]
        public string? BatchNo { get; set; }
    }

    public class SupplierPayment
    {
        public int Id { get; set; }

        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; } = null!;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMode { get; set; } = string.Empty; // Cash, Cheque, Bank Transfer, UPI

        [StringLength(100)]
        public string? TransactionReference { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [StringLength(300)]
        public string? Notes { get; set; }

        public int? PurchaseId { get; set; }
        public Purchase? Purchase { get; set; }

        public int CreatedByUserId { get; set; }
        public User CreatedBy { get; set; } = null!;
    }
}
