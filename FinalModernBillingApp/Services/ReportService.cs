using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ModernBillingApp.Data;
using ModernBillingApp.Models;

// This service replaces all the query logic from frmSales.cs, frmCustomer.cs, etc.
public class ReportService
{
    private readonly AppDbContext _context;

    public ReportService(AppDbContext context)
    {
        _context = context;
    }

    // This replaces the logic in frmSales.cs
    public async Task<List<BillItem>> GetSalesLog(DateTime fromDate, DateTime toDate)
    {
        // Gets all individual items sold between two dates
        return await _context.BillItems
            .Include(bi => bi.Bill) // Include Bill details
            .Include(bi => bi.Product) // Include Product details
            .Where(bi => bi.Bill.BDate >= fromDate && bi.Bill.BDate <= toDate)
            .OrderByDescending(bi => bi.Bill.BDate)
            .ToListAsync();
    }

    // This replaces the "Profit Details" tab in frmSales.cs
    public async Task<List<ProfitReportModel>> GetProfitReport(DateTime fromDate, DateTime toDate)
    {
        return await _context.BillItems
            .Where(bi => bi.Bill.BDate >= fromDate && bi.Bill.BDate <= toDate)
            .Select(bi => new ProfitReportModel
            {
                BillNo = bi.Bill.BillNo,
                BDate = bi.Bill.BDate,
                PName = bi.PName,
                PQty = (double)bi.PQty,
                SPrice = bi.SPrice,
                PPrice = bi.PPrice,
                // This is the profit calculation
                Profit = (bi.SPrice - bi.PPrice) * (decimal)bi.PQty
            })
            .ToListAsync();
    }

    // --- PASTE THIS CODE ---

    // This replaces the logic in frmStock.cs
    public async Task<List<StockLedger>> GetStockLedgerReport(DateTime fromDate, DateTime toDate)
    {
        // Gets all stock-in entries between two dates
        return await _context.StockLedgers
            .Include(s => s.Product)    // Include Product details
            .Include(s => s.Supplier)  // Include Supplier details
            .Where(s => s.Date >= fromDate && s.Date <= toDate && s.QtyAdded > 0) // Only get stock-in
            .OrderByDescending(s => s.Date)
            .ToListAsync();
    }

    // --- END OF PASTE ---

    // --- PASTE THIS CODE ---

    // This replaces the logic in frmCustomer.cs
    public async Task<List<Bill>> GetCustomerReport(DateTime fromDate, DateTime toDate)
    {
        // Gets all bills between two dates, including the Customer info
        return await _context.Bills
            .Include(b => b.Customer) // Include Customer details
            .Where(b => b.BDate >= fromDate && b.BDate <= toDate)
            .OrderByDescending(b => b.BDate)
            .ToListAsync();
    }

    // --- END OF PASTE ---

    // === COMPREHENSIVE REPORTS ===

    // Purchase Report
    public async Task<List<PurchaseReportModel>> GetPurchaseReport(DateTime fromDate, DateTime toDate, int? supplierId = null)
    {
        var query = _context.PurchaseEntries
            .Include(p => p.Supplier)
            .Include(p => p.Items)
            .Where(p => p.PurchaseDate >= fromDate && p.PurchaseDate <= toDate);

        if (supplierId.HasValue)
            query = query.Where(p => p.SupplierId == supplierId.Value);

        var purchases = await query.OrderByDescending(p => p.PurchaseDate).ToListAsync();

        return purchases.Select(p => new PurchaseReportModel
        {
            PurchaseNo = p.PurchaseNo,
            PurchaseDate = p.PurchaseDate,
            SupplierName = p.SupplierName ?? p.Supplier?.VName ?? "N/A",
            SupplierGSTIN = p.SupplierGSTIN ?? p.Supplier?.VGST,
            SubTotal = p.SubTotal,
            CGSTAmount = p.CGSTAmount,
            SGSTAmount = p.SGSTAmount,
            IGSTAmount = p.IGSTAmount,
            TotalGST = p.TotalGST,
            GrandTotal = p.GrandTotal,
            PaidAmount = p.PaidAmount,
            BalanceAmount = p.BalanceAmount,
            PaymentStatus = p.PaymentStatus,
            ItemCount = p.Items.Count
        }).ToList();
    }

    // Supplier Report
    public async Task<List<SupplierReportModel>> GetSupplierReport()
    {
        return await _context.Suppliers
            .Select(s => new SupplierReportModel
            {
                SupplierId = s.Id,
                SupplierName = s.VName ?? "N/A",
                Contact = s.VContact,
                City = s.VCity,
                State = s.VState,
                GSTIN = s.VGST,
                OutstandingBalance = s.OutstandingBalance,
                TotalPurchases = _context.PurchaseEntries
                    .Where(p => p.SupplierId == s.Id)
                    .Sum(p => p.GrandTotal),
                TotalPaid = _context.SupplierPayments
                    .Where(p => p.SupplierId == s.Id)
                    .Sum(p => p.Amount)
            })
            .ToListAsync();
    }

    // Customer Report
    public async Task<List<CustomerReportModel>> GetCustomerReport()
    {
        return await _context.Customers
            .Select(c => new CustomerReportModel
            {
                CustomerId = c.Id,
                CustomerName = c.CName ?? "N/A",
                Contact = c.CContact,
                City = c.CCity,
                State = c.CState,
                GSTIN = c.CGST,
                OutstandingBalance = c.OutstandingBalance,
                TotalPurchases = _context.Bills
                    .Where(b => b.CustomerId == c.Id)
                    .Sum(b => b.GrandTotal),
                TotalPaid = _context.CustomerPaymentEnhanced
                    .Where(p => p.CustomerId == c.Id)
                    .Sum(p => p.Amount),
                LoyaltyPoints = c.Points,
                TotalBills = _context.Bills.Count(b => b.CustomerId == c.Id)
            })
            .ToListAsync();
    }

    // Sales Summary Report
    public async Task<SalesSummaryReport> GetSalesSummaryReport(DateTime fromDate, DateTime toDate)
    {
        var bills = await _context.Bills
            .Include(b => b.Items)
            .Where(b => b.BDate >= fromDate && b.BDate <= toDate)
            .ToListAsync();

        return new SalesSummaryReport
        {
            FromDate = fromDate,
            ToDate = toDate,
            TotalBills = bills.Count,
            TotalSales = bills.Sum(b => b.GrandTotal),
            TotalCGST = bills.Sum(b => b.CGSTAmount),
            TotalSGST = bills.Sum(b => b.SGSTAmount),
            TotalIGST = bills.Sum(b => b.IGSTAmount),
            TotalGST = bills.Sum(b => b.GstAmount),
            TotalReceived = bills.Sum(b => b.PayAmt),
            TotalOutstanding = bills.Sum(b => b.BalAmt),
            TotalItemsSold = bills.Sum(b => b.Items.Sum(i => (double)i.PQty)),
            AverageBillValue = bills.Any() ? bills.Average(b => b.GrandTotal) : 0
        };
    }
}

// Report Models
public class PurchaseReportModel
{
    public string PurchaseNo { get; set; } = "";
    public DateTime PurchaseDate { get; set; }
    public string SupplierName { get; set; } = "";
    public string? SupplierGSTIN { get; set; }
    public decimal SubTotal { get; set; }
    public decimal CGSTAmount { get; set; }
    public decimal SGSTAmount { get; set; }
    public decimal IGSTAmount { get; set; }
    public decimal TotalGST { get; set; }
    public decimal GrandTotal { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal BalanceAmount { get; set; }
    public string PaymentStatus { get; set; } = "";
    public int ItemCount { get; set; }
}

public class SupplierReportModel
{
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = "";
    public string? Contact { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? GSTIN { get; set; }
    public decimal OutstandingBalance { get; set; }
    public decimal TotalPurchases { get; set; }
    public decimal TotalPaid { get; set; }
}

public class CustomerReportModel
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = "";
    public string? Contact { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? GSTIN { get; set; }
    public decimal OutstandingBalance { get; set; }
    public decimal TotalPurchases { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal LoyaltyPoints { get; set; }
    public int TotalBills { get; set; }
}

public class SalesSummaryReport
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int TotalBills { get; set; }
    public decimal TotalSales { get; set; }
    public decimal TotalCGST { get; set; }
    public decimal TotalSGST { get; set; }
    public decimal TotalIGST { get; set; }
    public decimal TotalGST { get; set; }
    public decimal TotalReceived { get; set; }
    public decimal TotalOutstanding { get; set; }
    public double TotalItemsSold { get; set; }
    public decimal AverageBillValue { get; set; }
}

// A helper class to hold the profit report data
public class ProfitReportModel
{
    public string BillNo { get; set; }
    public DateTime BDate { get; set; }
    public string? PName { get; set; }
    public double PQty { get; set; }
    [Column(TypeName = "decimal(18, 2)")]
    public decimal SPrice { get; set; }
    [Column(TypeName = "decimal(18, 2)")]
    public decimal PPrice { get; set; }
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Profit { get; set; }
}
