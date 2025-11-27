using Microsoft.EntityFrameworkCore;
using ModernBillingApp.Data;
using ModernBillingApp.Models;

namespace ModernBillingApp.Services
{
    public class StaffService
    {
        private readonly AppDbContext _context;
        private readonly SessionService _sessionService;

        public StaffService(AppDbContext context, SessionService sessionService)
        {
            _context = context;
            _sessionService = sessionService;
        }

        public async Task<List<Staff>> GetAllStaffAsync()
        {
            try
            {
                return await _context.Staff
                    .Include(s => s.User)
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.FullName)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<Staff>();
            }
        }

        public async Task<Staff?> GetStaffByIdAsync(int staffId)
        {
            try
            {
                return await _context.Staff
                    .Include(s => s.User)
                    .Include(s => s.Attendances)
                    .Include(s => s.Salaries)
                    .Include(s => s.Leaves)
                    .FirstOrDefaultAsync(s => s.Id == staffId);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Staff?> GetStaffByEmployeeIdAsync(string employeeId)
        {
            try
            {
                return await _context.Staff
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.EmployeeId == employeeId && s.IsActive);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> CreateStaffAsync(Staff staff)
        {
            try
            {
                // Check if employee ID already exists
                var existingStaff = await _context.Staff
                    .AnyAsync(s => s.EmployeeId == staff.EmployeeId);

                if (existingStaff)
                {
                    return false;
                }

                staff.CreatedDate = DateTime.Now;
                staff.IsActive = true;
                staff.Status = "Active";

                _context.Staff.Add(staff);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateStaffAsync(Staff staff)
        {
            try
            {
                var existingStaff = await _context.Staff.FindAsync(staff.Id);
                if (existingStaff == null)
                {
                    return false;
                }

                // Check if employee ID is being changed and already exists
                if (staff.EmployeeId != existingStaff.EmployeeId)
                {
                    var employeeIdExists = await _context.Staff
                        .AnyAsync(s => s.EmployeeId == staff.EmployeeId && s.Id != staff.Id);

                    if (employeeIdExists)
                    {
                        return false;
                    }
                }

                // Update all fields
                existingStaff.EmployeeId = staff.EmployeeId;
                existingStaff.FullName = staff.FullName;
                existingStaff.Phone = staff.Phone;
                existingStaff.Email = staff.Email;
                existingStaff.Address = staff.Address;
                existingStaff.City = staff.City;
                existingStaff.State = staff.State;
                existingStaff.PinCode = staff.PinCode;
                existingStaff.DateOfBirth = staff.DateOfBirth;
                existingStaff.Gender = staff.Gender;
                existingStaff.Designation = staff.Designation;
                existingStaff.Department = staff.Department;
                existingStaff.JoiningDate = staff.JoiningDate;
                existingStaff.BasicSalary = staff.BasicSalary;
                existingStaff.HRA = staff.HRA;
                existingStaff.Allowances = staff.Allowances;
                existingStaff.Deductions = staff.Deductions;
                existingStaff.NetSalary = staff.NetSalary;
                existingStaff.BankAccountNumber = staff.BankAccountNumber;
                existingStaff.BankName = staff.BankName;
                existingStaff.IFSCCode = staff.IFSCCode;
                existingStaff.PANNumber = staff.PANNumber;
                existingStaff.AadharNumber = staff.AadharNumber;
                existingStaff.PFNumber = staff.PFNumber;
                existingStaff.ESINumber = staff.ESINumber;
                existingStaff.EmploymentType = staff.EmploymentType;
                existingStaff.Status = staff.Status;
                existingStaff.Notes = staff.Notes;
                existingStaff.EmergencyContactName = staff.EmergencyContactName;
                existingStaff.EmergencyContactPhone = staff.EmergencyContactPhone;
                existingStaff.EmergencyContactRelation = staff.EmergencyContactRelation;
                existingStaff.LastUpdated = DateTime.Now;

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteStaffAsync(int staffId)
        {
            try
            {
                var staff = await _context.Staff
                    .Include(s => s.Attendances)
                    .Include(s => s.Salaries)
                    .Include(s => s.Leaves)
                    .FirstOrDefaultAsync(s => s.Id == staffId);

                if (staff == null)
                {
                    return false;
                }

                // Check if staff has any attendance, salary, or leave records
                if (staff.Attendances.Any() || staff.Salaries.Any() || staff.Leaves.Any())
                {
                    // Soft delete - mark as inactive
                    staff.IsActive = false;
                    staff.Status = "Terminated";
                    staff.ResignationDate = DateTime.Now;
                    staff.LastUpdated = DateTime.Now;
                }
                else
                {
                    // Hard delete if no records
                    _context.Staff.Remove(staff);
                }

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<Staff>> SearchStaffAsync(string searchTerm)
        {
            try
            {
                searchTerm = searchTerm.ToLower();

                return await _context.Staff
                    .Include(s => s.User)
                    .Where(s => s.IsActive &&
                        (s.FullName.ToLower().Contains(searchTerm) ||
                         s.EmployeeId.ToLower().Contains(searchTerm) ||
                         (s.Phone != null && s.Phone.Contains(searchTerm)) ||
                         (s.Email != null && s.Email.ToLower().Contains(searchTerm))))
                    .OrderBy(s => s.FullName)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<Staff>();
            }
        }

        public async Task<bool> MarkAttendanceAsync(int staffId, DateTime date, TimeOnly? inTime, TimeOnly? outTime, string status, string? remarks = null)
        {
            try
            {
                var existingAttendance = await _context.StaffAttendances
                    .FirstOrDefaultAsync(a => a.StaffId == staffId && a.AttendanceDate.Date == date.Date);

                if (existingAttendance != null)
                {
                    // Update existing attendance
                    existingAttendance.InTime = inTime;
                    existingAttendance.OutTime = outTime;
                    existingAttendance.Status = status;
                    existingAttendance.Remarks = remarks;

                    if (inTime.HasValue && outTime.HasValue)
                    {
                        existingAttendance.WorkingHours = outTime.Value.ToTimeSpan() - inTime.Value.ToTimeSpan();
                    }
                }
                else
                {
                    // Create new attendance
                    var attendance = new StaffAttendance
                    {
                        StaffId = staffId,
                        AttendanceDate = date.Date,
                        InTime = inTime,
                        OutTime = outTime,
                        Status = status,
                        Remarks = remarks,
                        IsManualEntry = true,
                        MarkedByUserId = _sessionService.CurrentUserId,
                        CreatedDate = DateTime.Now
                    };

                    if (inTime.HasValue && outTime.HasValue)
                    {
                        attendance.WorkingHours = outTime.Value.ToTimeSpan() - inTime.Value.ToTimeSpan();
                    }

                    _context.StaffAttendances.Add(attendance);
                }

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<StaffAttendance>> GetStaffAttendanceAsync(int staffId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                return await _context.StaffAttendances
                    .Include(a => a.MarkedBy)
                    .Where(a => a.StaffId == staffId &&
                           a.AttendanceDate >= fromDate.Date &&
                           a.AttendanceDate <= toDate.Date)
                    .OrderByDescending(a => a.AttendanceDate)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<StaffAttendance>();
            }
        }

        public async Task<bool> ProcessSalaryAsync(int staffId, DateTime salaryMonth, decimal basicSalary, decimal hra, decimal allowances, decimal overtime, decimal bonus, decimal pf, decimal esi, decimal tds, decimal advance, decimal otherDeductions, int workingDays, int presentDays, int absentDays, string? notes = null)
        {
            try
            {
                var existingSalary = await _context.StaffSalaries
                    .FirstOrDefaultAsync(s => s.StaffId == staffId &&
                                        s.SalaryMonth.Year == salaryMonth.Year &&
                                        s.SalaryMonth.Month == salaryMonth.Month);

                if (existingSalary != null)
                {
                    return false;
                }

                var grossSalary = basicSalary + hra + allowances + overtime + bonus;
                var totalDeductions = pf + esi + tds + advance + otherDeductions;
                var netSalary = grossSalary - totalDeductions;

                var salary = new StaffSalary
                {
                    StaffId = staffId,
                    SalaryMonth = salaryMonth,
                    BasicSalary = basicSalary,
                    HRA = hra,
                    Allowances = allowances,
                    Overtime = overtime,
                    Bonus = bonus,
                    GrossSalary = grossSalary,
                    PF = pf,
                    ESI = esi,
                    TDS = tds,
                    Advance = advance,
                    OtherDeductions = otherDeductions,
                    TotalDeductions = totalDeductions,
                    NetSalary = netSalary,
                    WorkingDays = workingDays,
                    PresentDays = presentDays,
                    AbsentDays = absentDays,
                    Status = "Processed",
                    ProcessedByUserId = _sessionService.CurrentUserId ?? 1,
                    CreatedDate = DateTime.Now,
                    Notes = notes
                };

                _context.StaffSalaries.Add(salary);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<StaffSalary>> GetStaffSalariesAsync(int staffId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var query = _context.StaffSalaries
                    .Include(s => s.Staff)
                    .Include(s => s.ProcessedBy)
                    .Where(s => s.StaffId == staffId);

                if (fromDate.HasValue)
                    query = query.Where(s => s.SalaryMonth >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(s => s.SalaryMonth <= toDate.Value);

                return await query
                    .OrderByDescending(s => s.SalaryMonth)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<StaffSalary>();
            }
        }

        public async Task<bool> PaySalaryAsync(int salaryId, DateTime paymentDate, string paymentMode, string? transactionRef = null)
        {
            try
            {
                var salary = await _context.StaffSalaries.FindAsync(salaryId);
                if (salary == null)
                {
                    return false;
                }

                salary.Status = "Paid";
                salary.PaidDate = paymentDate;
                salary.PaymentMode = paymentMode;
                salary.PaymentReference = transactionRef;

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<int> GetTotalStaffCountAsync()
        {
            return await _context.Staff.CountAsync(s => s.IsActive);
        }

        public async Task<int> GetPresentTodayCountAsync()
        {
            var today = DateTime.Today;
            return await _context.StaffAttendances
                .CountAsync(a => a.AttendanceDate == today && a.Status == "Present");
        }

        public async Task<decimal> GetTotalMonthlySalaryAsync()
        {
            return await _context.Staff
                .Where(s => s.IsActive)
                .SumAsync(s => s.NetSalary);
        }
    }
}
