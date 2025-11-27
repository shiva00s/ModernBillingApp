using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewModernBillingApp.Models
{
    public class Staff
    {
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string EmployeeId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

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

        public DateTime DateOfBirth { get; set; }

        [StringLength(20)]
        public string? Gender { get; set; }

        [StringLength(50)]
        public string? Designation { get; set; }

        [StringLength(50)]
        public string? Department { get; set; }

        public DateTime JoiningDate { get; set; } = DateTime.Now;

        public DateTime? ResignationDate { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal BasicSalary { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal HRA { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Allowances { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Deductions { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal NetSalary { get; set; }

        [StringLength(50)]
        public string? BankAccountNumber { get; set; }

        [StringLength(100)]
        public string? BankName { get; set; }

        [StringLength(20)]
        public string? IFSCCode { get; set; }

        [StringLength(15)]
        public string? PANNumber { get; set; }

        [StringLength(20)]
        public string? AadharNumber { get; set; }

        [StringLength(20)]
        public string? PFNumber { get; set; }

        [StringLength(20)]
        public string? ESINumber { get; set; }

        [StringLength(50)]
        public string EmploymentType { get; set; } = "Permanent"; // Permanent, Contract, Temporary

        [StringLength(50)]
        public string Status { get; set; } = "Active"; // Active, Inactive, Terminated

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? LastUpdated { get; set; }

        [StringLength(200)]
        public string? Notes { get; set; }

        [StringLength(200)]
        public string? EmergencyContactName { get; set; }

        [StringLength(15)]
        public string? EmergencyContactPhone { get; set; }

        [StringLength(100)]
        public string? EmergencyContactRelation { get; set; }

        // User relationship - if staff has system access
        public int? UserId { get; set; }
        public User? User { get; set; }

        // Navigation properties
        public ICollection<StaffAttendance> Attendances { get; set; } = new List<StaffAttendance>();
        public ICollection<StaffSalary> Salaries { get; set; } = new List<StaffSalary>();
        public ICollection<StaffLeave> Leaves { get; set; } = new List<StaffLeave>();
    }

    public class StaffAttendance
    {
        public int Id { get; set; }

        public int StaffId { get; set; }
        public Staff Staff { get; set; } = null!;

        public DateTime AttendanceDate { get; set; } = DateTime.Today;

        public TimeOnly? InTime { get; set; }

        public TimeOnly? OutTime { get; set; }

        public TimeSpan? WorkingHours { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Present"; // Present, Absent, Half Day, Late, Holiday

        [StringLength(200)]
        public string? Remarks { get; set; }

        public bool IsManualEntry { get; set; } = false;

        public int? MarkedByUserId { get; set; }
        public User? MarkedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }

    public class StaffSalary
    {
        public int Id { get; set; }

        public int StaffId { get; set; }
        public Staff Staff { get; set; } = null!;

        public DateTime SalaryMonth { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal BasicSalary { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal HRA { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Allowances { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Overtime { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Bonus { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal GrossSalary { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PF { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal ESI { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TDS { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Advance { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal OtherDeductions { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalDeductions { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal NetSalary { get; set; }

        public int WorkingDays { get; set; } = 0;

        public int PresentDays { get; set; } = 0;

        public int AbsentDays { get; set; } = 0;

        [StringLength(50)]
        public string Status { get; set; } = "Draft"; // Draft, Processed, Paid

        public DateTime? PaidDate { get; set; }

        [StringLength(50)]
        public string? PaymentMode { get; set; }

        [StringLength(100)]
        public string? PaymentReference { get; set; }

        public int ProcessedByUserId { get; set; }
        public User ProcessedBy { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [StringLength(300)]
        public string? Notes { get; set; }
    }

    public class StaffLeave
    {
        public int Id { get; set; }

        public int StaffId { get; set; }
        public Staff Staff { get; set; } = null!;

        [StringLength(50)]
        public string LeaveType { get; set; } = string.Empty; // Casual, Sick, Earned, Maternity, etc.

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int TotalDays { get; set; }

        [StringLength(300)]
        public string? Reason { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

        public DateTime AppliedDate { get; set; } = DateTime.Now;

        public DateTime? ApprovedDate { get; set; }

        public int? ApprovedByUserId { get; set; }
        public User? ApprovedBy { get; set; }

        [StringLength(200)]
        public string? ApprovalRemarks { get; set; }

        [StringLength(200)]
        public string? DocumentPath { get; set; } // For medical certificates, etc.

        public bool IsHalfDay { get; set; } = false;

        [StringLength(20)]
        public string? HalfDayType { get; set; } // Morning, Evening

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }

    public class StaffDocument
    {
        public int Id { get; set; }

        public int StaffId { get; set; }
        public Staff Staff { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string DocumentType { get; set; } = string.Empty; // Resume, ID Proof, Address Proof, etc.

        [Required]
        [StringLength(200)]
        public string DocumentName { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string FilePath { get; set; } = string.Empty;

        [StringLength(20)]
        public string? FileSize { get; set; }

        [StringLength(50)]
        public string? FileType { get; set; }

        public DateTime UploadDate { get; set; } = DateTime.Now;

        public int UploadedByUserId { get; set; }
        public User UploadedBy { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        [StringLength(200)]
        public string? Description { get; set; }
    }
}
