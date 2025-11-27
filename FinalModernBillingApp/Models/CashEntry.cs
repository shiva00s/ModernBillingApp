using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModernBillingApp.Models
{
    public class CashEntry
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string EntryName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string EntryType { get; set; } = string.Empty; // Income, Expense

        [StringLength(100)]
        public string? Category { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        public DateTime EntryDate { get; set; } = DateTime.Now;

        [StringLength(300)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? PaymentMode { get; set; } = "Cash";

        [StringLength(100)]
        public string? TransactionReference { get; set; }

        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public int? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        [StringLength(100)]
        public string? ReceivedFrom { get; set; }

        [StringLength(100)]
        public string? PaidTo { get; set; }

        public bool IsActive { get; set; } = true;

        public int CreatedByUserId { get; set; }
        public User CreatedBy { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? LastUpdated { get; set; }

        [StringLength(200)]
        public string? Notes { get; set; }

        // Backward compatibility properties (not mapped to database)
        [NotMapped]
        public string? CEName
        {
            get => EntryName;
            set => EntryName = value ?? string.Empty;
        }

        [NotMapped]
        public string? CEType
        {
            get => EntryType;
            set => EntryType = value ?? string.Empty;
        }

        [NotMapped]
        public DateTime Date
        {
            get => EntryDate;
            set => EntryDate = value;
        }

        [NotMapped]
        public string? Remark
        {
            get => Description;
            set => Description = value;
        }

        [NotMapped]
        public string? CEID { get; set; }
    }
}
