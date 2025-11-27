using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModernBillingApp.Models
{
    // This is the MASTER record for the bill.
    // It replaces the "header" parts of your old table.
    public class Bill
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string BillNo { get; set; } = string.Empty;

        public DateTime BDate { get; set; } = DateTime.Now; // Merged from BDate and BillDate

        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }

        // Copied customer details (for historical records)
        [StringLength(100)]
        public string? CName { get; set; } // Renamed from CName to CustomerName in NewModernBillingApp, keeping CName for existing
        [StringLength(15)]
        public string? CContact { get; set; } // Renamed from CContact to CustomerPhone in NewModernBillingApp, keeping CContact for existing
        [StringLength(300)]
        public string? CustomerAddress { get; set; } // Added from NewModernBillingApp

        public string? SalesPerson { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMode { get; set; } = "Cash"; // Cash, Card, UPI, NEFT, Credit

        [StringLength(50)]
        public string? UPIReference { get; set; } // From ModernBillingApp
        public string? PaymentDetails { get; set; } // From ModernBillingApp
        [StringLength(100)]
        public string? PaymentReference { get; set; } // Added from NewModernBillingApp

        [StringLength(50)]
        public string PaymentStatus { get; set; } = "Paid"; // Paid, Partial, Pending (Added from NewModernBillingApp)

        [Column(TypeName = "decimal(18, 2)")]
        public decimal SubTotal { get; set; } // Replaced TotalAmount (before tax) with SubTotal

        [Column(TypeName = "decimal(18, 2)")]
        public decimal DiscountAmount { get; set; } = 0; // Added from NewModernBillingApp

        [Column(TypeName = "decimal(8, 2)")]
        public decimal DiscountPercentage { get; set; } = 0; // Added from NewModernBillingApp

        [Column(TypeName = "decimal(18, 2)")]
        public decimal GstAmount { get; set; } // From ModernBillingApp, similar to GSTAmount in NewModernBillingApp

        // Indian GST Split
        [Column(TypeName = "decimal(18, 2)")]
        public decimal CGSTAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal SGSTAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal IGSTAmount { get; set; } = 0;

        public bool IsInterState { get; set; } = false;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; } // Replaced GrandTotal with TotalAmount (final)

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PaidAmount { get; set; } = 0; // Renamed PayAmt to PaidAmount, default 0 from NewModernBillingApp

        [Column(TypeName = "decimal(18, 2)")]
        public decimal BalanceAmount { get; set; } // Renamed BalAmt to BalanceAmount

        [StringLength(200)]
        public string? Notes { get; set; } // Added from NewModernBillingApp

        public bool IsPrinted { get; set; } = false; // Added from NewModernBillingApp

        public bool IsActive { get; set; } = true; // Added from NewModernBillingApp

        public int CreatedByUserId { get; set; } // Added from NewModernBillingApp
        public User CreatedBy { get; set; } = null!; // Added from NewModernBillingApp

        public DateTime CreatedDate { get; set; } = DateTime.Now; // Added from NewModernBillingApp

        public DateTime? LastUpdated { get; set; } // Added from NewModernBillingApp


        // This links the Bill to its list of items
        public ICollection<BillItem> Items { get; set; } = new List<BillItem>();
        public ICollection<CustomerPayment> Payments { get; set; } = new List<CustomerPayment>(); // Added from NewModernBillingApp

        // Backward compatibility properties (NotMapped)
        [NotMapped]
        public decimal GrandTotal
        {
            get => TotalAmount;
            set => TotalAmount = value;
        }

        [NotMapped]
        public decimal PayAmt
        {
            get => PaidAmount;
            set => PaidAmount = value;
        }

        [NotMapped]
        public decimal BalAmt
        {
            get => BalanceAmount;
            set => BalanceAmount = value;
        }
    }

    // This is the "child" table for each item on the bill.
    // This replaces the "line item" parts of your old table.
    public class BillItem
    {
        public int Id { get; set; }

        // This links back to the main Bill
        public int BillId { get; set; }
        public Bill Bill { get; set; } = null!; // Added null! from NewModernBillingApp

        // This links to the Product Catalog
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!; // Added null! from NewModernBillingApp

        // Details at the time of sale
        [StringLength(100)]
        public string ProductName { get; set; } = string.Empty; // Replaced PName
        [StringLength(50)]
        public string? ProductCode { get; set; } // Replaced PID
        [StringLength(20)]
        public string? HSNCode { get; set; } // Replaced HSN

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Quantity { get; set; } // Replaced PQty (double) with Quantity (decimal)

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Rate { get; set; } // Replaced SPrice with Rate

        [Column(TypeName = "decimal(18, 2)")]
        public decimal MRP { get; set; } // Added from NewModernBillingApp

        [Column(TypeName = "decimal(8, 2)")]
        public decimal DiscountPercentage { get; set; } = 0; // Added from NewModernBillingApp

        [Column(TypeName = "decimal(18, 2)")]
        public decimal DiscountAmount { get; set; } = 0; // Added from NewModernBillingApp

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PurchasePrice { get; set; } // Renamed PPrice to PurchasePrice

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
        public string Unit { get; set; } = "PCS";

        [StringLength(50)]
        public string? BatchNo { get; set; }

        public DateTime? ExpiryDate { get; set; }

        // Backward compatibility properties (NotMapped)
        [NotMapped]
        public string? PName
        {
            get => ProductName;
            set => ProductName = value ?? string.Empty;
        }

        [NotMapped]
        public string? PID
        {
            get => ProductCode;
            set => ProductCode = value;
        }

        [NotMapped]
        public string? HSN
        {
            get => HSNCode;
            set => HSNCode = value;
        }

        [NotMapped]
        public double PQty
        {
            get => (double)Quantity;
            set => Quantity = (decimal)value;
        }

        [NotMapped]
        public decimal SPrice
        {
            get => Rate;
            set => Rate = value;
        }

        [NotMapped]
        public decimal PPrice
        {
            get => PurchasePrice;
            set => PurchasePrice = value;
        }

        [NotMapped]
        public decimal Gst
        {
            get => GST;
            set => GST = value;
        }

        [NotMapped]
        public decimal GstAmount
        {
            get => GSTAmount;
            set => GSTAmount = value;
        }
    }
}
