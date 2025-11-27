using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModernBillingApp.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string? PID { get; set; }
        [Required]
        public string? PName { get; set; }
        public string? HSN { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal SPrice { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal MRP { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Gst { get; set; }
        public double CurrentStock { get; set; } = 0;
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public ICollection<StockLedger> StockLedgerEntries { get; set; } = new List<StockLedger>();
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
        public DateTime Date { get; set; }
        public string? BatchNo { get; set; }
        public string? PurNo { get; set; }
        public double QtyAdded { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PPrice { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }
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