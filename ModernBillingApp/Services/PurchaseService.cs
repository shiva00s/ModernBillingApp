using Microsoft.EntityFrameworkCore;
using ModernBillingApp.Data;
using ModernBillingApp.Models;

namespace ModernBillingApp.Services
{
    // Purchase Entry and Purchase Return Service
    public class PurchaseService
    {
        private readonly AppDbContext _context;
        private readonly IndianGSTService _gstService;

        public PurchaseService(AppDbContext context, IndianGSTService gstService)
        {
            _context = context;
            _gstService = gstService;
        }

        // Create Purchase Entry
        public async Task<PurchaseEntry> CreatePurchaseEntry(
            PurchaseEntry purchase,
            List<PurchaseItem> items,
            bool isInterState = false)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Calculate totals
                decimal subTotal = items.Sum(i => i.Total);
                decimal totalCGST = 0, totalSGST = 0, totalIGST = 0;

                foreach (var item in items)
                {
                    item.GSTAmount = (item.Total * item.GSTRate) / 100;
                    
                    var (cgst, sgst, igst) = _gstService.CalculateGSTSplit(
                        item.Total,
                        item.GSTRate,
                        isInterState
                    );
                    
                    item.PurchaseEntry = purchase;
                    totalCGST += cgst;
                    totalSGST += sgst;
                    totalIGST += igst;
                }

                purchase.SubTotal = subTotal;
                purchase.CGSTAmount = totalCGST;
                purchase.SGSTAmount = totalSGST;
                purchase.IGSTAmount = totalIGST;
                purchase.TotalGST = totalCGST + totalSGST + totalIGST;
                purchase.GrandTotal = subTotal + purchase.TotalGST;
                purchase.BalanceAmount = purchase.GrandTotal - purchase.PaidAmount;
                purchase.PaymentStatus = purchase.BalanceAmount == 0 ? "Paid" 
                    : (purchase.PaidAmount > 0 ? "Partial" : "Pending");
                purchase.PurchaseDate = DateTime.Now;

                // Get next purchase number
                if (string.IsNullOrEmpty(purchase.PurchaseNo))
                {
                    purchase.PurchaseNo = await GetNextPurchaseNo();
                }

                _context.PurchaseEntries.Add(purchase);
                await _context.SaveChangesAsync();

                // Add items
                foreach (var item in items)
                {
                    item.PurchaseEntryId = purchase.Id;
                    _context.PurchaseItems.Add(item);

                    // Update product stock
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        product.CurrentStock += item.Quantity;
                        
                        // Update product prices if needed
                        if (item.PurchasePrice > 0)
                        {
                            // You can update product purchase price here
                        }
                    }

                    // Add to stock ledger
                    var stockEntry = new StockLedger
                    {
                        Date = purchase.PurchaseDate,
                        ProductId = item.ProductId,
                        QtyAdded = item.Quantity,
                        PPrice = item.PurchasePrice,
                        SupplierId = purchase.SupplierId,
                        PurNo = purchase.PurchaseNo,
                        BatchNo = item.BatchNo
                    };
                    _context.StockLedgers.Add(stockEntry);
                }

                // Update supplier balance
                var supplier = await _context.Suppliers.FindAsync(purchase.SupplierId);
                if (supplier != null)
                {
                    supplier.OutstandingBalance += purchase.BalanceAmount;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return purchase;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Get next purchase number
        public async Task<string> GetNextPurchaseNo()
        {
            var maxNo = await _context.PurchaseEntries
                .Select(p => p.PurchaseNo)
                .OrderByDescending(n => n)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(maxNo))
                return $"PUR-{DateTime.Now:yyyyMMdd}-001";

            // Extract number and increment
            var parts = maxNo.Split('-');
            if (parts.Length >= 3 && int.TryParse(parts[2], out int lastNum))
            {
                return $"{parts[0]}-{parts[1]}-{(lastNum + 1):D3}";
            }

            return $"PUR-{DateTime.Now:yyyyMMdd}-001";
        }

        // Create Purchase Return
        public async Task<PurchaseReturn> CreatePurchaseReturn(
            PurchaseReturn purchaseReturn,
            List<PurchaseReturnItem> items)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                purchaseReturn.ReturnDate = DateTime.Now;
                purchaseReturn.ReturnNo = await GetNextReturnNo();
                purchaseReturn.TotalAmount = items.Sum(i => i.ReturnAmount);

                _context.PurchaseReturns.Add(purchaseReturn);
                await _context.SaveChangesAsync();

                foreach (var item in items)
                {
                    item.PurchaseReturnId = purchaseReturn.Id;
                    _context.PurchaseReturnItems.Add(item);

                    // Reduce product stock
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        product.CurrentStock = Math.Max(0, product.CurrentStock - item.ReturnQuantity);
                    }

                    // Add negative stock ledger entry
                    var stockEntry = new StockLedger
                    {
                        Date = purchaseReturn.ReturnDate,
                        ProductId = item.ProductId,
                        QtyAdded = -item.ReturnQuantity,
                        PPrice = item.PurchaseItem.PurchasePrice,
                        SupplierId = purchaseReturn.SupplierId,
                        PurNo = $"RET-{purchaseReturn.ReturnNo}"
                    };
                    _context.StockLedgers.Add(stockEntry);
                }

                // Update supplier balance (reduce what we owe)
                var supplier = await _context.Suppliers.FindAsync(purchaseReturn.SupplierId);
                if (supplier != null)
                {
                    supplier.OutstandingBalance = Math.Max(0, 
                        supplier.OutstandingBalance - purchaseReturn.TotalAmount);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return purchaseReturn;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task<string> GetNextReturnNo()
        {
            var maxNo = await _context.PurchaseReturns
                .Select(r => r.ReturnNo)
                .OrderByDescending(n => n)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(maxNo))
                return $"PRET-{DateTime.Now:yyyyMMdd}-001";

            var parts = maxNo.Split('-');
            if (parts.Length >= 3 && int.TryParse(parts[2], out int lastNum))
            {
                return $"{parts[0]}-{parts[1]}-{(lastNum + 1):D3}";
            }

            return $"PRET-{DateTime.Now:yyyyMMdd}-001";
        }

        // Get purchase entries
        public async Task<List<PurchaseEntry>> GetPurchaseEntries(
            int? supplierId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var query = _context.PurchaseEntries
                .Include(p => p.Supplier)
                .Include(p => p.Items)
                .AsQueryable();

            if (supplierId.HasValue)
                query = query.Where(p => p.SupplierId == supplierId.Value);

            if (fromDate.HasValue)
                query = query.Where(p => p.PurchaseDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(p => p.PurchaseDate <= toDate.Value);

            return await query.OrderByDescending(p => p.PurchaseDate).ToListAsync();
        }
    }
}

