// This C# class replaces your old "Employee_Entry" SQL table
public class Employee
{
    public int Id { get; set; } // Replaces AutoID
    public string? UserId { get; set; } // From your old table
    public string? EmpNo { get; set; }
    public string? EmpName { get; set; }
    public string? Gender { get; set; }
    public string? Address { get; set; }
    public string? EmpMobile { get; set; }
    public DateTime? DOJ { get; set; } // Date of Joining
    public string? Mar_Sts { get; set; } // Marital Status
    public DateTime? DOB { get; set; } // Date of Birth
    public string? Department { get; set; }
    public string? Job_Type { get; set; }
    public double? Salary { get; set; }
}