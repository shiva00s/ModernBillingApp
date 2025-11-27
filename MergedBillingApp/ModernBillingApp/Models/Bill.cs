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
    public string BillNo { get; set; }
    public DateTime BDate { get; set; } // Bill Date

    // Link to the Customer
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    // Copied customer details (for historical records)
    public string? CName { get; set; }
    public string? CContact { get; set; }

    public string? SalesPerson { get; set; }
    public string? PaymentMode { get; set; } // Cash, Card, UPI, NEFT, etc.

    [StringLength(50)]
    public string? UPIReference { get; set; } // UPI transaction reference
    public string? PaymentDetails { get; set; } // Additional payment info

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalAmount { get; set; } // Total *before* tax

    [Column(TypeName = "decimal(18, 2)")]
    public decimal GstAmount { get; set; }

    // Indian GST Split
    [Column(TypeName = "decimal(18, 2)")]
    public decimal CGSTAmount { get; set; } = 0;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal SGSTAmount { get; set; } = 0;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal IGSTAmount { get; set; } = 0;

    public bool IsInterState { get; set; } = false;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal GrandTotal { get; set; } // Total + Tax

    [Column(TypeName = "decimal(18, 2)")]
    public decimal PayAmt { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal BalAmt { get; set; }

    // This links the Bill to its list of items
    public ICollection<BillItem> Items { get; set; } = new List<BillItem>();
    }

    // This is the "child" table for each item on the bill.
    // This replaces the "line item" parts of your old table.
    public class BillItem
{
    public int Id { get; set; }

    // This links back to the main Bill
    public int BillId { get; set; }
    public Bill Bill { get; set; }

    // This links to the Product Catalog
    public int ProductId { get; set; }
    public Product Product { get; set; }

    // Details at the time of sale
    public string? PID { get; set; }
    public string? PName { get; set; }
    public string? HSN { get; set; }
    public double PQty { get; set; } // Quantity sold

    [Column(TypeName = "decimal(18, 2)")]
    public decimal SPrice { get; set; } // Sales Price

    [Column(TypeName = "decimal(18, 2)")]
    public decimal PPrice { get; set; } // Purchase Price (for profit)

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Gst { get; set; } // GST %

    [Column(TypeName = "decimal(18, 2)")]
    public decimal GstAmount { get; set; } // GST amount for this line

    // Indian GST Split
    [Column(TypeName = "decimal(18, 2)")]
    public decimal CGSTAmount { get; set; } = 0;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal SGSTAmount { get; set; } = 0;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal IGSTAmount { get; set; } = 0;

    [StringLength(20)]
    public string? Unit { get; set; } // Unit sold (PCS, KG, LTR, etc.)

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Total { get; set; } // SPrice * PQty
    }
}
