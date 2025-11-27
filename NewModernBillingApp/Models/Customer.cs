using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewModernBillingApp.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(15)]
        public string? Phone { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(300)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        [StringLength(50)]
        public string? State { get; set; }

        [StringLength(10)]
        public string? PinCode { get; set; }

        [StringLength(15)]
        public string? GSTNumber { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal CreditLimit { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal OutstandingAmount { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? LastUpdated { get; set; }

        [StringLength(50)]
        public string? CustomerType { get; set; } = "Regular"; // Regular, Premium, VIP

        [StringLength(200)]
        public string? Notes { get; set; }

        // Navigation properties
        public ICollection<Bill> Bills { get; set; } = new List<Bill>();
        public ICollection<CustomerPayment> Payments { get; set; } = new List<CustomerPayment>();
    }

    public class CustomerPayment
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMode { get; set; } = string.Empty; // Cash, Card, UPI, Bank Transfer

        [StringLength(100)]
        public string? TransactionReference { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [StringLength(300)]
        public string? Notes { get; set; }

        public int? BillId { get; set; }
        public Bill? Bill { get; set; }

        public int CreatedByUserId { get; set; }
        public User CreatedBy { get; set; } = null!;
    }
}
