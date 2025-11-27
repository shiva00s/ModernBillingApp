using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModernBillingApp.Models
{
    public class Supplier
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string? VID { get; set; }

        [Required(ErrorMessage = "Supplier name is required.")]
        [StringLength(100)]
        public string? VName { get; set; }

        [StringLength(300)]
        public string? VAddress { get; set; }

        [StringLength(15)]
        public string? VContact { get; set; }

        [StringLength(50)]
        public string? VCity { get; set; }

        [StringLength(50)]
        public string? VState { get; set; }

        [StringLength(15)]
        public string? VGST { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? VEmail { get; set; }

        [StringLength(10)]
        public string? VPincode { get; set; }

        public DateTime? VDate { get; set; }

        [StringLength(100)]
        public string? ContactPerson { get; set; }

        [StringLength(50)]
        public string? PANNumber { get; set; }

        // Supplier balance tracking
        [Column(TypeName = "decimal(18, 2)")]
        public decimal OutstandingBalance { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal OutstandingAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal CreditLimit { get; set; } = 0;

        public int PaymentTerms { get; set; } = 30; // Days

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? LastUpdated { get; set; }

        [StringLength(200)]
        public string? Notes { get; set; }

        // Links to related entities
        public ICollection<StockLedger> StockEntries { get; set; } = new List<StockLedger>();
        public ICollection<SupplierPayment> Payments { get; set; } = new List<SupplierPayment>();
        public ICollection<PurchaseEntry> PurchaseEntries { get; set; } = new List<PurchaseEntry>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<CashEntry> CashEntries { get; set; } = new List<CashEntry>();

        // Backward compatibility properties
        [NotMapped]
        public string? Name
        {
            get => VName;
            set => VName = value;
        }

        [NotMapped]
        public string? Phone
        {
            get => VContact;
            set => VContact = value;
        }

        [NotMapped]
        public string? Email
        {
            get => VEmail;
            set => VEmail = value;
        }

        [NotMapped]
        public string? Address
        {
            get => VAddress;
            set => VAddress = value;
        }

        [NotMapped]
        public string? City
        {
            get => VCity;
            set => VCity = value;
        }

        [NotMapped]
        public string? State
        {
            get => VState;
            set => VState = value;
        }

        [NotMapped]
        public string? GSTNumber
        {
            get => VGST;
            set => VGST = value;
        }

        [NotMapped]
        public string? PinCode
        {
            get => VPincode;
            set => VPincode = value;
        }
    }
}
