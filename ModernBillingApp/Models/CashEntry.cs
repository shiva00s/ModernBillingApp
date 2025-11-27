using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModernBillingApp.Models
{
    // This C# class replaces your "CashEntry" SQL table
    public class CashEntry
    {
        public int Id { get; set; } // Replaces AutoID
        public string? CEID { get; set; } // Your old ID

        [Required]
        public string? CEName { get; set; } // Name of the entry (e.g., "Rent", "Supplies")

        [Required]
        public string? CEType { get; set; } // "Income" or "Expense"

        public DateTime Date { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        public string? Remark { get; set; }
    }
}
