using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ModernBillingApp.Models;

// This C# class replaces your "Supplier_Entry" SQL table
public class Supplier
{
    public int Id { get; set; } // Replaces AutoID
    public string? VID { get; set; } // Your old Supplier ID

    [Required(ErrorMessage = "Supplier name is required.")]
    public string? VName { get; set; }
    public string? VAddress { get; set; }
    public string? VContact { get; set; }
    public string? VCity { get; set; }
    public string? VState { get; set; }
    public string? VGST { get; set; }
    public string? VEmail { get; set; }
    public string? VPincode { get; set; }
    public DateTime? VDate { get; set; }
    
    // Supplier balance tracking
    [Column(TypeName = "decimal(18, 2)")]
    public decimal OutstandingBalance { get; set; } = 0; // Amount owed to supplier
    
    [Column(TypeName = "decimal(18, 2)")]
    public decimal CreditLimit { get; set; } = 0; // Maximum credit from supplier

    // Links to related entities
    public ICollection<StockLedger> StockEntries { get; set; } = new List<StockLedger>();
    public ICollection<SupplierPayment> Payments { get; set; } = new List<SupplierPayment>();
    public ICollection<PurchaseEntry> PurchaseEntries { get; set; } = new List<PurchaseEntry>();
}