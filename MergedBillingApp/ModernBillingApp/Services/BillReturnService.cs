using Microsoft.EntityFrameworkCore;
using ModernBillingApp.Data;
using ModernBillingApp.Models;

namespace ModernBillingApp.Services
{
    // Bill Return Service
    public class BillReturnService
    {
        private readonly AppDbContext _context;
        private readonly IndianGSTService _gstService;

        public BillReturnService(AppDbContext context, IndianGSTService gstService)
        {
            _context = context;
            _gstService = gstService;
        }

        // Create Bill Return
        public async Task<BillReturn> CreateBillReturn(
            int originalBillId,
            List<BillReturnItem> items,
            string reason,
            string? paymentMode = null,
            string? remarks = null)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var originalBill = await _context.Bills
                    .Include(b => b.Items)
                    .Include(b => b.Customer)
                    .FirstOrDefaultAsync(b => b.Id == originalBillId);

                if (originalBill == null)
                    throw new InvalidOperationException("Original bill not found");

                var billReturn = new BillReturn
                {
                    OriginalBillId = originalBillId,
                    OriginalBillNo = originalBill.BillNo,
                    ReturnDate = DateTime.Now,
                    ReturnNo = await GetNextReturnNo(),
                    CustomerId = originalBill.CustomerId,
                    PaymentMode = paymentMode ?? originalBill.PaymentMode,
                    Reason = reason,
                    Remarks = remarks,
                    CreatedDate = DateTime.Now
                };

                decimal totalReturn = 0;
                decimal totalCGST = 0, totalSGST = 0, totalIGST = 0;

                foreach (var item in items)
                {
                    var originalItem = originalBill.Items.FirstOrDefault(i => i.Id == item.OriginalBillItemId);
                    if (originalItem == null)
                        continue;

                    // Validate return quantity
                    if (item.ReturnQuantity > originalItem.PQty)
                        throw new InvalidOperationException(
                            $"Cannot return more than sold. Sold: {originalItem.PQty}, Returning: {item.ReturnQuantity}");

                    item.ReturnPrice = originalItem.SPrice;
                    item.ReturnAmount = item.ReturnPrice * (decimal)item.ReturnQuantity;
                    
                    // Calculate GST for return (proportional to quantity returned)
                    decimal returnGSTAmount = (originalItem.GstAmount * (decimal)item.ReturnQuantity) / (decimal)originalItem.PQty;
                    
                    var (cgst, sgst, igst) = _gstService.CalculateGSTSplit(
                        item.ReturnAmount,
                        originalItem.Gst,
                        originalBill.IsInterState
                    );
                    
                    item.CGSTAmount = cgst;
                    item.SGSTAmount = sgst;
                    item.IGSTAmount = igst;
                    
                    totalReturn += item.ReturnAmount;
                    totalCGST += cgst;
                    totalSGST += sgst;
                    totalIGST += igst;

                    item.BillReturnId = billReturn.Id;
                    item.BillReturn = billReturn;

                    // Restore product stock
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        product.CurrentStock += item.ReturnQuantity;
                    }

                    // Add positive stock ledger entry
                    var stockEntry = new StockLedger
                    {
                        Date = billReturn.ReturnDate,
                        ProductId = item.ProductId,
                        QtyAdded = item.ReturnQuantity,
                        PPrice = originalItem.PPrice,
                        PurNo = $"RET-{billReturn.ReturnNo}"
                    };
                    _context.StockLedgers.Add(stockEntry);
                }

                billReturn.ReturnAmount = totalReturn;
                billReturn.CGSTAmount = totalCGST;
                billReturn.SGSTAmount = totalSGST;
                billReturn.IGSTAmount = totalIGST;

                _context.BillReturns.Add(billReturn);
                await _context.SaveChangesAsync();

                // Update customer balance (reduce what customer owes)
                if (originalBill.CustomerId.HasValue)
                {
                    var customer = await _context.Customers.FindAsync(originalBill.CustomerId.Value);
                    if (customer != null)
                    {
                        decimal refundAmount = totalReturn + totalCGST + totalSGST + totalIGST;
                        customer.OutstandingBalance = Math.Max(0, 
                            customer.OutstandingBalance - refundAmount);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return billReturn;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task<string> GetNextReturnNo()
        {
            var maxNo = await _context.BillReturns
                .Select(r => r.ReturnNo)
                .OrderByDescending(n => n)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(maxNo))
                return $"BRET-{DateTime.Now:yyyyMMdd}-001";

            var parts = maxNo.Split('-');
            if (parts.Length >= 3 && int.TryParse(parts[2], out int lastNum))
            {
                return $"{parts[0]}-{parts[1]}-{(lastNum + 1):D3}";
            }

            return $"BRET-{DateTime.Now:yyyyMMdd}-001";
        }

        // Get bill returns
        public async Task<List<BillReturn>> GetBillReturns(
            int? customerId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var query = _context.BillReturns
                .Include(r => r.OriginalBill)
                .Include(r => r.Customer)
                .AsQueryable();

            if (customerId.HasValue)
                query = query.Where(r => r.CustomerId == customerId);

            if (fromDate.HasValue)
                query = query.Where(r => r.ReturnDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(r => r.ReturnDate <= toDate.Value);

            return await query.OrderByDescending(r => r.ReturnDate).ToListAsync();
        }
    }
}

