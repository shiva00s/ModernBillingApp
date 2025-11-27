using System.ComponentModel.DataAnnotations.Schema;

// This C# class replaces your "Stock_Return_SpaProduct" SQL table
public class StockReturn
{
    public int Id { get; set; } // Replaces AutoID
    public string? PID { get; set; }
    public string? PName { get; set; }
    public string? Scat { get; set; }
    public string? HSN { get; set; }
    public double Qty { get; set; } // Old Qty
    public double TQty { get; set; } // Returned Qty
    public double CQty { get; set; } // New Qty

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