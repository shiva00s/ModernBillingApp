using Microsoft.EntityFrameworkCore;
using ModernBillingApp.Data;
using ModernBillingApp.Models;

namespace ModernBillingApp.Services
{
    // Comprehensive Payment Management Service
    public class PaymentService
    {
        private readonly AppDbContext _context;

        public PaymentService(AppDbContext context)
        {
            _context = context;
        }

        // === CUSTOMER PAYMENT METHODS ===

        // Record customer payment (partial or full)
        public async Task<CustomerPaymentEnhanced> RecordCustomerPayment(
            int customerId,
            decimal amount,
            string paymentMode,
            string? billNo = null,
            int? billId = null,
            string? referenceNo = null,
            string? remarks = null)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var customer = await _context.Customers.FindAsync(customerId);
                if (customer == null)
                    throw new InvalidOperationException("Customer not found");

                decimal billAmount = 0;
                decimal previousPaid = 0;
                bool isFullPayment = false;

                // If bill-wise payment
                if (billId.HasValue)
                {
                    var bill = await _context.Bills
                        .Include(b => b.Items)
                        .FirstOrDefaultAsync(b => b.Id == billId.Value);

                    if (bill == null)
                        throw new InvalidOperationException("Bill not found");

                    billAmount = bill.GrandTotal;
                    
                    // Calculate previous payments for this bill
                    previousPaid = await _context.CustomerPaymentEnhanced
                        .Where(p => p.BillId == billId && p.Id != 0) // Exclude current payment
                        .SumAsync(p => p.CurrentPayment);

                    decimal remaining = billAmount - previousPaid;
                    isFullPayment = amount >= remaining;
                }
                else
                {
                    // General payment (not bill-specific)
                    billAmount = customer.OutstandingBalance;
                    previousPaid = 0;
                    isFullPayment = amount >= billAmount;
                }

                var payment = new CustomerPaymentEnhanced
                {
                    CustomerId = customerId,
                    PaymentDate = DateTime.Now,
                    PaymentMode = paymentMode,
                    Amount = amount,
                    BillNo = billNo,
                    BillId = billId,
                    BillAmount = billAmount,
                    PreviousPaid = previousPaid,
                    CurrentPayment = amount,
                    RemainingBalance = billAmount - previousPaid - amount,
                    IsFullPayment = isFullPayment,
                    ReferenceNo = referenceNo,
                    Remarks = remarks,
                    CreatedDate = DateTime.Now
                };

                _context.CustomerPaymentEnhanced.Add(payment);

                // Update customer balance
                customer.OutstandingBalance = Math.Max(0, customer.OutstandingBalance - amount);

                // Update bill balance if bill-specific
                if (billId.HasValue)
                {
                    var bill = await _context.Bills.FindAsync(billId.Value);
                    if (bill != null)
                    {
                        bill.BalAmt = Math.Max(0, bill.BalAmt - amount);
                        bill.PayAmt += amount;
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return payment;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Get customer payment history
        public async Task<List<CustomerPaymentEnhanced>> GetCustomerPayments(
            int customerId,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var query = _context.CustomerPaymentEnhanced
                .Where(p => p.CustomerId == customerId);

            if (fromDate.HasValue)
                query = query.Where(p => p.PaymentDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(p => p.PaymentDate <= toDate.Value);

            return await query.OrderByDescending(p => p.PaymentDate).ToListAsync();
        }

        // Get bill-wise payment summary
        public async Task<List<BillPaymentSummary>> GetBillPaymentSummary(int? customerId = null)
        {
            var query = _context.Bills
                .Include(b => b.Customer)
                .Where(b => b.BalAmt > 0 || customerId == null || b.CustomerId == customerId)
                .Select(b => new BillPaymentSummary
                {
                    BillId = b.Id,
                    BillNo = b.BillNo,
                    BillDate = b.BDate,
                    CustomerId = b.CustomerId,
                    CustomerName = b.CName ?? "Walk-in",
                    GrandTotal = b.GrandTotal,
                    PaidAmount = b.PayAmt,
                    BalanceAmount = b.BalAmt,
                    PaymentStatus = b.BalAmt == 0 ? "Paid" : (b.PayAmt > 0 ? "Partial" : "Pending")
                });

            return await query.ToListAsync();
        }

        // === SUPPLIER PAYMENT METHODS ===

        // Record supplier payment
        public async Task<SupplierPayment> RecordSupplierPayment(
            int supplierId,
            decimal amount,
            string paymentMode,
            string? purchaseNo = null,
            int? purchaseEntryId = null,
            string? referenceNo = null,
            string? remarks = null)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var supplier = await _context.Suppliers.FindAsync(supplierId);
                if (supplier == null)
                    throw new InvalidOperationException("Supplier not found");

                var payment = new SupplierPayment
                {
                    SupplierId = supplierId,
                    PaymentDate = DateTime.Now,
                    PaymentMode = paymentMode,
                    Amount = amount,
                    PurchaseNo = purchaseNo,
                    PurchaseEntryId = purchaseEntryId,
                    ReferenceNo = referenceNo,
                    Remarks = remarks,
                    CreatedDate = DateTime.Now
                };

                _context.SupplierPayments.Add(payment);

                // Update supplier balance
                supplier.OutstandingBalance = Math.Max(0, supplier.OutstandingBalance - amount);

                // Update purchase entry if bill-wise payment
                if (purchaseEntryId.HasValue)
                {
                    var purchase = await _context.PurchaseEntries.FindAsync(purchaseEntryId.Value);
                    if (purchase != null)
                    {
                        purchase.PaidAmount += amount;
                        purchase.BalanceAmount = purchase.GrandTotal - purchase.PaidAmount;
                        purchase.PaymentStatus = purchase.BalanceAmount == 0 ? "Paid" 
                            : (purchase.PaidAmount > 0 ? "Partial" : "Pending");
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return payment;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Get supplier payment history
        public async Task<List<SupplierPayment>> GetSupplierPayments(
            int supplierId,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var query = _context.SupplierPayments
                .Where(p => p.SupplierId == supplierId);

            if (fromDate.HasValue)
                query = query.Where(p => p.PaymentDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(p => p.PaymentDate <= toDate.Value);

            return await query.OrderByDescending(p => p.PaymentDate).ToListAsync();
        }

        // === PAYMENT REPORTS ===

        // Weekly payment summary
        public async Task<PaymentSummary> GetWeeklyPaymentSummary(DateTime weekStart)
        {
            var weekEnd = weekStart.AddDays(7);
            
            var customerPayments = await _context.CustomerPaymentEnhanced
                .Where(p => p.PaymentDate >= weekStart && p.PaymentDate < weekEnd)
                .SumAsync(p => p.Amount);

            var supplierPayments = await _context.SupplierPayments
                .Where(p => p.PaymentDate >= weekStart && p.PaymentDate < weekEnd)
                .SumAsync(p => p.Amount);

            return new PaymentSummary
            {
                Period = $"Week of {weekStart:MMM dd, yyyy}",
                CustomerPaymentsReceived = customerPayments,
                SupplierPaymentsMade = supplierPayments,
                NetCashFlow = customerPayments - supplierPayments
            };
        }

        // Monthly payment summary
        public async Task<PaymentSummary> GetMonthlyPaymentSummary(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1);

            var customerPayments = await _context.CustomerPaymentEnhanced
                .Where(p => p.PaymentDate >= startDate && p.PaymentDate < endDate)
                .SumAsync(p => p.Amount);

            var supplierPayments = await _context.SupplierPayments
                .Where(p => p.PaymentDate >= startDate && p.PaymentDate < endDate)
                .SumAsync(p => p.Amount);

            return new PaymentSummary
            {
                Period = $"{startDate:MMMM yyyy}",
                CustomerPaymentsReceived = customerPayments,
                SupplierPaymentsMade = supplierPayments,
                NetCashFlow = customerPayments - supplierPayments
            };
        }
    }

    // Helper classes for reports
    public class BillPaymentSummary
    {
        public int BillId { get; set; }
        public string BillNo { get; set; } = "";
        public DateTime BillDate { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; } = "";
        public decimal GrandTotal { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        public string PaymentStatus { get; set; } = "";
    }

    public class PaymentSummary
    {
        public string Period { get; set; } = "";
        public decimal CustomerPaymentsReceived { get; set; }
        public decimal SupplierPaymentsMade { get; set; }
        public decimal NetCashFlow { get; set; }
    }
}

