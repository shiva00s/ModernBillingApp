using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ModernBillingApp.Models
{
    public class LegacyBill
    {
        public int Id { get; set; }
        [Required]
        public string BillNo { get; set; }
        public DateTime BDate { get; set; }
        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public string? CName { get; set; }
        public string? CContact { get; set; }
        public string? SalesPerson { get; set; }
        public string? PaymentMode { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal GstAmount { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal GrandTotal { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PayAmt { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal BalAmt { get; set; }
        public ICollection<LegacyBillItem> Items { get; set; } = new List<LegacyBillItem>();
    }

    public class LegacyBillItem
    {
        public int Id { get; set; }
        public int BillId { get; set; }
        public LegacyBill Bill { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string? PID { get; set; }
        public string? PName { get; set; }
        public string? HSN { get; set; }
        public double PQty { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal SPrice { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PPrice { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Gst { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal GstAmount { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Total { get; set; }
    }
}
