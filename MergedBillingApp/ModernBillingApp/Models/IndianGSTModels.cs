using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModernBillingApp.Models
{
    // Enhanced GST Model for Indian Market - CGST/SGST Split
    public class IndianGSTConfig
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(15)]
        public string? GSTIN { get; set; } // Shop GSTIN
        
        [StringLength(200)]
        public string? ShopName { get; set; }
        
        [StringLength(500)]
        public string? ShopAddress { get; set; }
        
        [StringLength(50)]
        public string? State { get; set; }
        
        [StringLength(10)]
        public string? Pincode { get; set; }
        
        [StringLength(15)]
        public string? Phone { get; set; }
        
        [StringLength(100)]
        public string? Email { get; set; }
        
        public bool IsInterState { get; set; } = false; // For IGST calculation
    }

    // Enhanced Product GST Model
    public class ProductGST
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal GSTRate { get; set; } // Total GST (e.g., 18%)
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal CGSTRate { get; set; } // CGST (half of GST for intra-state)
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal SGSTRate { get; set; } // SGST (half of GST for intra-state)
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal IGSTRate { get; set; } // IGST (for inter-state)
        
        [StringLength(10)]
        public string? HSNCode { get; set; }
        
        [StringLength(50)]
        public string? SACCode { get; set; } // For services
    }
}

