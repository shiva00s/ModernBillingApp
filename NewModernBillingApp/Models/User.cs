using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewModernBillingApp.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [StringLength(100)]
        public string? FullName { get; set; }

        [StringLength(15)]
        public string? Phone { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? LastLoginDate { get; set; }

        // Role relationship
        public int RoleId { get; set; }
        public UserRole Role { get; set; } = null!;

        // Navigation properties
        public ICollection<Bill> Bills { get; set; } = new List<Bill>();
        public ICollection<CashEntry> CashEntries { get; set; } = new List<CashEntry>();
    }

    public class UserRole
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string RoleName { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; }

        public bool IsSystemRole { get; set; } = false;

        // Navigation properties
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<RolePermission> Permissions { get; set; } = new List<RolePermission>();
    }

    public class RolePermission
    {
        public int Id { get; set; }

        public int RoleId { get; set; }
        public UserRole Role { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string PermissionKey { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; }

        public bool CanCreate { get; set; } = false;
        public bool CanRead { get; set; } = true;
        public bool CanUpdate { get; set; } = false;
        public bool CanDelete { get; set; } = false;
    }
}
