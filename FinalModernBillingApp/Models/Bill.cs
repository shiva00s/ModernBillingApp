using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModernBillingApp.Models
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

        // Backward compatibility properties (not mapped to database)
        [NotMapped]
        public decimal GstAmount
        {
            get => GSTAmount;
            set => GSTAmount = value;
        }

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

        [NotMapped]
        public string? CName
        {
            get => CustomerName;
            set => CustomerName = value;
        }

        [NotMapped]
        public string? CContact
        {
            get => CustomerPhone;
            set => CustomerPhone = value;
        }

        [NotMapped]
        public string? UPIReference
        {
            get => PaymentReference;
            set => PaymentReference = value;
        }

        [NotMapped]
        public string? TransactionReference
        {
            get => PaymentReference;
            set => PaymentReference = value;
        }

        [NotMapped]
        public DateTime BDate
        {
            get => BillDate;
            set => BillDate = value;
        }
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

        // Backward compatibility properties (not mapped to database)
        [NotMapped]
        public decimal PQty
        {
            get => Quantity;
            set => Quantity = value;
        }

        [NotMapped]
        public decimal GstAmount
        {
            get => GSTAmount;
            set => GSTAmount = value;
        }

        [NotMapped]
        public decimal Gst
        {
            get => GST;
            set => GST = value;
        }

        [NotMapped]
        public decimal PPrice
        {
            get => Product?.PurchasePrice ?? 0;
        }

        // Additional backward compatibility properties
        [NotMapped]
        public string? PID
        {
            get => ProductCode;
            set => ProductCode = value;
        }

        [NotMapped]
        public string? PName
        {
            get => ProductName;
            set => ProductName = value ?? string.Empty;
        }

        [NotMapped]
        public string? HSN
        {
            get => HSNCode;
            set => HSNCode = value;
        }

        [NotMapped]
        public decimal SPrice
        {
            get => Rate;
            set => Rate = value;
        }
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
        public decimal CGSTAmount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal SGSTAmount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal IGSTAmount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal ReturnAmount { get; set; }

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

        // Backward compatibility properties
        [NotMapped]
        public string? OriginalBillNo
        {
            get => OriginalBill?.BillNo;
        }

        [NotMapped]
        public string? PaymentMode
        {
            get => RefundMode;
            set => RefundMode = value ?? "Cash";
        }

        [NotMapped]
        public string? Remarks
        {
            get => Notes;
            set => Notes = value;
        }
    }

    public class BillReturnItem
    {
        public int Id { get; set; }

        public int BillReturnId { get; set; }
        public BillReturn BillReturn { get; set; } = null!;

        public int BillItemId { get; set; }
        public BillItem OriginalBillItem { get; set; } = null!;

        [NotMapped]
        public int OriginalBillItemId
        {
            get => BillItemId;
            set => BillItemId = value;
        }

        [NotMapped]
        public decimal ReturnPrice
        {
            get => Rate;
            set => Rate = value;
        }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        [StringLength(100)]
        public string ProductName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10, 2)")]
        public decimal ReturnQuantity { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Rate { get; set; }

        [Column(TypeName = "decimal(8, 2)")]
        public decimal GST { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal GSTAmount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal CGSTAmount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal SGSTAmount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal IGSTAmount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal ReturnAmount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Total { get; set; }

        [StringLength(100)]
        public string? Reason { get; set; }
    }
}
