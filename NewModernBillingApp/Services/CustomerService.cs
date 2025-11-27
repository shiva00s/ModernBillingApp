using Microsoft.EntityFrameworkCore;
using NewModernBillingApp.Data;
using NewModernBillingApp.Models;

namespace NewModernBillingApp.Services
{
    public class CustomerService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CustomerService> _logger;
        private readonly SessionService _sessionService;

        public CustomerService(AppDbContext context, ILogger<CustomerService> logger, SessionService sessionService)
        {
            _context = context;
            _logger = logger;
            _sessionService = sessionService;
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            try
            {
                return await _context.Customers
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all customers");
                return new List<Customer>();
            }
        }

        public async Task<Customer?> GetCustomerByIdAsync(int customerId)
        {
            try
            {
                return await _context.Customers
                    .Include(c => c.Bills)
                    .Include(c => c.Payments)
                    .FirstOrDefaultAsync(c => c.Id == customerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer with ID '{CustomerId}'", customerId);
                return null;
            }
        }

        public async Task<Customer?> GetCustomerByPhoneAsync(string phone)
        {
            try
            {
                return await _context.Customers
                    .FirstOrDefaultAsync(c => c.Phone == phone && c.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer with phone '{Phone}'", phone);
                return null;
            }
        }

        public async Task<bool> CreateCustomerAsync(Customer customer)
        {
            try
            {
                // Check if customer with same phone already exists
                if (!string.IsNullOrEmpty(customer.Phone))
                {
                    var existingCustomer = await _context.Customers
                        .AnyAsync(c => c.Phone == customer.Phone && c.IsActive);

                    if (existingCustomer)
                    {
                        _logger.LogWarning("Customer creation failed: Phone '{Phone}' already exists", customer.Phone);
                        return false;
                    }
                }

                customer.CreatedDate = DateTime.Now;
                customer.IsActive = true;

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Customer '{CustomerName}' created successfully", customer.Name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer '{CustomerName}'", customer.Name);
                return false;
            }
        }

        public async Task<bool> UpdateCustomerAsync(Customer customer)
        {
            try
            {
                var existingCustomer = await _context.Customers.FindAsync(customer.Id);
                if (existingCustomer == null)
                {
                    _logger.LogWarning("Customer update failed: Customer with ID '{CustomerId}' not found", customer.Id);
                    return false;
                }

                // Check if phone is being changed and already exists
                if (!string.IsNullOrEmpty(customer.Phone) && customer.Phone != existingCustomer.Phone)
                {
                    var phoneExists = await _context.Customers
                        .AnyAsync(c => c.Phone == customer.Phone && c.Id != customer.Id && c.IsActive);

                    if (phoneExists)
                    {
                        _logger.LogWarning("Customer update failed: Phone '{Phone}' already exists", customer.Phone);
                        return false;
                    }
                }

                existingCustomer.Name = customer.Name;
                existingCustomer.Phone = customer.Phone;
                existingCustomer.Email = customer.Email;
                existingCustomer.Address = customer.Address;
                existingCustomer.City = customer.City;
                existingCustomer.State = customer.State;
                existingCustomer.PinCode = customer.PinCode;
                existingCustomer.GSTNumber = customer.GSTNumber;
                existingCustomer.CreditLimit = customer.CreditLimit;
                existingCustomer.CustomerType = customer.CustomerType;
                existingCustomer.Notes = customer.Notes;
                existingCustomer.LastUpdated = DateTime.Now;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Customer '{CustomerName}' updated successfully", customer.Name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer '{CustomerName}'", customer.Name);
                return false;
            }
        }

        public async Task<bool> DeleteCustomerAsync(int customerId)
        {
            try
            {
                var customer = await _context.Customers
                    .Include(c => c.Bills)
                    .FirstOrDefaultAsync(c => c.Id == customerId);

                if (customer == null)
                {
                    _logger.LogWarning("Customer deletion failed: Customer with ID '{CustomerId}' not found", customerId);
                    return false;
                }

                // Check if customer has any bills
                if (customer.Bills.Any())
                {
                    // Soft delete - just mark as inactive
                    customer.IsActive = false;
                    customer.LastUpdated = DateTime.Now;
                }
                else
                {
                    // Hard delete if no bills
                    _context.Customers.Remove(customer);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Customer '{CustomerName}' deleted successfully", customer.Name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer with ID '{CustomerId}'", customerId);
                return false;
            }
        }

        public async Task<List<Customer>> SearchCustomersAsync(string searchTerm)
        {
            try
            {
                searchTerm = searchTerm.ToLower();

                return await _context.Customers
                    .Where(c => c.IsActive &&
                        (c.Name.ToLower().Contains(searchTerm) ||
                         c.Phone!.Contains(searchTerm) ||
                         c.Email!.ToLower().Contains(searchTerm)))
                    .OrderBy(c => c.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching customers with term '{SearchTerm}'", searchTerm);
                return new List<Customer>();
            }
        }

        public async Task<bool> AddCustomerPaymentAsync(int customerId, decimal amount, string paymentMode, string? transactionReference = null, string? notes = null, int? billId = null)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(customerId);
                if (customer == null)
                {
                    _logger.LogWarning("Payment failed: Customer with ID '{CustomerId}' not found", customerId);
                    return false;
                }

                var payment = new CustomerPayment
                {
                    CustomerId = customerId,
                    Amount = amount,
                    PaymentMode = paymentMode,
                    TransactionReference = transactionReference,
                    Notes = notes,
                    BillId = billId,
                    PaymentDate = DateTime.Now,
                    CreatedByUserId = _sessionService.CurrentUserId ?? 1
                };

                _context.CustomerPayments.Add(payment);

                // Update customer outstanding amount
                customer.OutstandingAmount -= amount;
                if (customer.OutstandingAmount < 0)
                    customer.OutstandingAmount = 0;

                customer.LastUpdated = DateTime.Now;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Payment of {Amount} added for customer '{CustomerName}'", amount, customer.Name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding payment for customer '{CustomerId}'", customerId);
                return false;
            }
        }

        public async Task<List<CustomerPayment>> GetCustomerPaymentsAsync(int customerId)
        {
            try
            {
                return await _context.CustomerPayments
                    .Include(p => p.Bill)
                    .Include(p => p.CreatedBy)
                    .Where(p => p.CustomerId == customerId)
                    .OrderByDescending(p => p.PaymentDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payments for customer '{CustomerId}'", customerId);
                return new List<CustomerPayment>();
            }
        }

        public async Task<decimal> GetCustomerOutstandingAsync(int customerId)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(customerId);
                return customer?.OutstandingAmount ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting outstanding amount for customer '{CustomerId}'", customerId);
                return 0;
            }
        }
    }

    public class StaffService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<StaffService> _logger;
        private readonly SessionService _sessionService;

        public StaffService(AppDbContext context, ILogger<StaffService> logger, SessionService sessionService)
        {
            _context = context;
            _logger = logger;
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all staff");
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting staff with ID '{StaffId}'", staffId);
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting staff with employee ID '{EmployeeId}'", employeeId);
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
                    _logger.LogWarning("Staff creation failed: Employee ID '{EmployeeId}' already exists", staff.EmployeeId);
                    return false;
                }

                staff.CreatedDate = DateTime.Now;
                staff.IsActive = true;
                staff.Status = "Active";

                _context.Staff.Add(staff);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Staff '{StaffName}' created successfully with Employee ID '{EmployeeId}'", staff.FullName, staff.EmployeeId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating staff '{StaffName}'", staff.FullName);
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
                    _logger.LogWarning("Staff update failed: Staff with ID '{StaffId}' not found", staff.Id);
                    return false;
                }

                // Check if employee ID is being changed and already exists
                if (staff.EmployeeId != existingStaff.EmployeeId)
                {
                    var employeeIdExists = await _context.Staff
                        .AnyAsync(s => s.EmployeeId == staff.EmployeeId && s.Id != staff.Id);

                    if (employeeIdExists)
                    {
                        _logger.LogWarning("Staff update failed: Employee ID '{EmployeeId}' already exists", staff.EmployeeId);
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

                _logger.LogInformation("Staff '{StaffName}' updated successfully", staff.FullName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating staff '{StaffName}'", staff.FullName);
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
                    _logger.LogWarning("Staff deletion failed: Staff with ID '{StaffId}' not found", staffId);
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

                _logger.LogInformation("Staff '{StaffName}' deleted successfully", staff.FullName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting staff with ID '{StaffId}'", staffId);
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
                         s.Phone!.Contains(searchTerm) ||
                         s.Email!.ToLower().Contains(searchTerm)))
                    .OrderBy(s => s.FullName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching staff with term '{SearchTerm}'", searchTerm);
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

                _logger.LogInformation("Attendance marked for staff '{StaffId}' on '{Date}'", staffId, date.Date);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking attendance for staff '{StaffId}' on '{Date}'", staffId, date.Date);
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting attendance for staff '{StaffId}'", staffId);
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
                    _logger.LogWarning("Salary already processed for staff '{StaffId}' for month '{SalaryMonth}'", staffId, salaryMonth.ToString("yyyy-MM"));
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

                _logger.LogInformation("Salary processed for staff '{StaffId}' for month '{SalaryMonth}'", staffId, salaryMonth.ToString("yyyy-MM"));
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing salary for staff '{StaffId}'", staffId);
                return false;
            }
        }
    }
}
