using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModernBillingApp.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? PID { get; set; }

        [Required]
        [StringLength(100)]
        public string? PName { get; set; }

        [StringLength(20)]
        public string? HSN { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal SPrice { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal MRP { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Gst { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PurchasePrice { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal CurrentStock { get; set; } = 0;

        [Column(TypeName = "decimal(10, 2)")]
        public decimal MinimumStock { get; set; } = 0;

        [Column(TypeName = "decimal(10, 2)")]
        public decimal MaximumStock { get; set; } = 0;

        [StringLength(50)]
        public string? Barcode { get; set; }

        [StringLength(20)]
        public string? Unit { get; set; } = "PCS";

        public DateTime? ExpiryDate { get; set; }

        [StringLength(50)]
        public string? BatchNo { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? LastUpdated { get; set; }

        [StringLength(300)]
        public string? Description { get; set; }

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public int? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        // Navigation properties
        public ICollection<StockLedger> StockLedgerEntries { get; set; } = new List<StockLedger>();
        public ICollection<BillItem> BillItems { get; set; } = new List<BillItem>();

        // Backward compatibility properties (not mapped)
        [NotMapped]
        public string? Name
        {
            get => PName;
            set => PName = value;
        }

        [NotMapped]
        public string? ProductCode
        {
            get => PID;
            set => PID = value;
        }

        [NotMapped]
        public decimal SellingPrice
        {
            get => SPrice;
            set => SPrice = value;
        }

        [NotMapped]
        public decimal GST
        {
            get => Gst;
            set => Gst = value;
        }

        [NotMapped]
        public string? HSNCode
        {
            get => HSN;
            set => HSN = value;
        }
    }

    public class Category
    {
        public int Id { get; set; }
        public string? CatName { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
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

        public int? ReferenceId { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public string? BatchNo { get; set; }

        public string? PurNo { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal QtyAdded { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Quantity { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PPrice { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Rate { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal StockBefore { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal StockAfter { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [StringLength(200)]
        public string? Notes { get; set; }

        public int? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        public int? CreatedByUserId { get; set; }
        public User? CreatedBy { get; set; }
    }

    public class StockReturn
    {
        public int Id { get; set; }
        public string? PID { get; set; }
        public string? PName { get; set; }
        public string? Scat { get; set; }
        public string? HSN { get; set; }
        public double Qty { get; set; }
        public double TQty { get; set; }
        public double CQty { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PPrice { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal SPrice { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal MRP { get; set; }
        public double Gst { get; set; }
        public string? Size { get; set; }
        public string? Type { get; set; }
        public string? VID { get; set; }
        public string? VName { get; set; }
        public string? PurNo { get; set; }
        public DateTime? Date { get; set; }
        public string? Remarks { get; set; }
        public string? BatchNo { get; set; }
    }
}
