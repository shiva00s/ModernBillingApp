using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModernBillingApp.Models
{
    // This C# class replaces your "Stock_Product_SpaProduct" table
    // This is the MASTER product list.
    public class Product
    {
        public int Id { get; set; } // Replaces AutoID

        [Required]
        public string? PID { get; set; } // Your old Product ID

        [Required]
        public string? PName { get; set; } // Product Name

        public string? HSN { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal SPrice { get; set; } // Sales Price

        [Column(TypeName = "decimal(18, 2)")]
        public decimal MRP { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Gst { get; set; }

        // Indian Market Enhancements
        [StringLength(50)]
        public string? Barcode { get; set; } // Barcode/EAN for scanning

        [StringLength(20)]
        public string? Unit { get; set; } = "PCS"; // Unit: PCS, KG, LTR, etc.

        public DateTime? ExpiryDate { get; set; } // For expiry tracking

        [StringLength(50)]
        public string? BatchNo { get; set; } // Batch number for tracking

        // --- FUTURE IMPROVEMENT ---
        // We store the CURRENT stock level here for fast lookups
        public double CurrentStock { get; set; } = 0;

        // This links to the Category
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        // This links to all the times this product was stocked
        public ICollection<StockLedger> StockLedgerEntries { get; set; } = new List<StockLedger>();
    }

    // This C# class replaces your "Stock_Category_SpaProduct" table
    public class Category
    {
        public int Id { get; set; }
        public string? CatName { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }

    // --- INDUSTRY STANDARD IMPROVEMENT ---
    // This new table creates a log of all stock you add.
    // This replaces the "inventory" part of your old stock table.
    public class StockLedger
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string? BatchNo { get; set; }
        public string? PurNo { get; set; } // Purchase Order No.
        public double QtyAdded { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PPrice { get; set; } // Purchase Price

        // This links to the Product
        public int ProductId { get; set; }
        public Product Product { get; set; }

        // This links to the Supplier
        public int? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }
    }
}
