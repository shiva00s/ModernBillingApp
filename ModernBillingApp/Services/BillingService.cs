using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using ModernBillingApp.Data;
using ModernBillingApp.Models;
using ModernBillingApp.Services;

// This service replaces all the query logic from frmBillingEntrySpaShop.cs
public class BillingService
{
    private readonly AppDbContext _context;

    public BillingService(AppDbContext context)
    {
        _context = context;
    }

    // Gets the next bill number
    public async Task<string> GetNextBillNo()
    {
        // This replaces your ScalerQuery for the bill number
        var maxBillNo = await _context.Bills
                              .Select(b => b.BillNo)
                              .MaxAsync();

        if (string.IsNullOrEmpty(maxBillNo))
        {
            return "1"; // Starting bill number
        }

        int nextBill = int.Parse(maxBillNo) + 1;
        return nextBill.ToString();
    }

    // This is the main "Save Bill" function with enhanced error handling
    public async Task<Bill> CreateBill(Bill bill, List<BillItem> items, IndianGSTService? gstService = null)
    {
        // Validate inputs
        if (bill == null)
            throw new ArgumentNullException(nameof(bill), "Bill cannot be null");
            
        if (items == null || !items.Any())
            throw new InvalidOperationException("Cannot create bill with no items");

        // This is a "Transaction"
        // It makes sure that BOTH the bill is saved AND stock is updated.
        // If one part fails, everything is rolled back. This is a FLAWLESS design.
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Calculate Indian GST split if service is provided
            if (gstService != null)
            {
                decimal totalCGST = 0, totalSGST = 0, totalIGST = 0;
                
                foreach (var item in items)
                {
                    var (cgst, sgst, igst) = gstService.CalculateGSTSplit(
                        item.Total, 
                        item.Gst, 
                        bill.IsInterState
                    );
                    
                    item.CGSTAmount = cgst;
                    item.SGSTAmount = sgst;
                    item.IGSTAmount = igst;
                    
                    totalCGST += cgst;
                    totalSGST += sgst;
                    totalIGST += igst;
                }
                
                bill.CGSTAmount = totalCGST;
                bill.SGSTAmount = totalSGST;
                bill.IGSTAmount = totalIGST;
            }

            // 1. Save the main Bill record
            bill.BDate = DateTime.Now;
            _context.Bills.Add(bill);
            await _context.SaveChangesAsync(); // Bill gets its new ID here

            // 2. Save all Bill Items and update stock
            foreach (var item in items)
            {
                // Validate stock availability
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                    throw new InvalidOperationException($"Product with ID {item.ProductId} not found");
                    
                if (product.CurrentStock < item.PQty)
                    throw new InvalidOperationException(
                        $"Insufficient stock for {product.PName}. Available: {product.CurrentStock}, Required: {item.PQty}");

                // Link the item to the bill
                item.BillId = bill.Id;
                item.Unit = product.Unit ?? "PCS"; // Copy unit from product
                _context.BillItems.Add(item);

                // 3. Update the Product's CurrentStock
                product.CurrentStock -= item.PQty;

                // 4. Add a negative entry to StockLedger for audit
                var stockLog = new StockLedger
                {
                    Date = DateTime.Now,
                    QtyAdded = -item.PQty, // Negative Qty for a sale
                    PPrice = item.PPrice, // Log purchase price for profit reports
                    ProductId = item.ProductId,
                    PurNo = $"BillNo-{bill.BillNo}",
                    BatchNo = product.BatchNo
                };
                _context.StockLedgers.Add(stockLog);
            }

            await _context.SaveChangesAsync(); // Save all items and stock changes

            // 4. Update customer balance if customer exists
            if (bill.CustomerId.HasValue)
            {
                var customer = await _context.Customers.FindAsync(bill.CustomerId.Value);
                if (customer != null)
                {
                    customer.OutstandingBalance += bill.BalAmt;
                }
            }

            await _context.SaveChangesAsync();

            // 5. Commit the transaction
            await transaction.CommitAsync();

            return bill;
        }
        catch (Exception ex)
        {
            // If anything fails, roll back all changes
            await transaction.RollbackAsync();
            
            // Log the error (you can enhance this with proper logging)
            throw new InvalidOperationException(
                $"Error creating bill: {ex.Message}", ex);
        }
    }
    // --- PASTE THIS NEW FUNCTION ---

    // Gets a single bill and all its items for printing
    public async Task<Bill> GetBillById(int billId)
    {
        return await _context.Bills
            .Include(b => b.Customer)
            .Include(b => b.Items)
                .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(b => b.Id == billId);
    }

    // --- END OF PASTE ---

}