using Microsoft.EntityFrameworkCore;
using NewModernBillingApp.Data;
using NewModernBillingApp.Models;

namespace NewModernBillingApp.Services
{
    public class ProductService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProductService> _logger;
        private readonly SessionService _sessionService;

        public ProductService(AppDbContext context, ILogger<ProductService> logger, SessionService sessionService)
        {
            _context = context;
            _logger = logger;
            _sessionService = sessionService;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            try
            {
                return await _context.Products
                    .Include(p => p.Supplier)
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all products");
                return new List<Product>();
            }
        }

        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            try
            {
                return await _context.Products
                    .Include(p => p.Supplier)
                    .Include(p => p.StockLedgers)
                    .FirstOrDefaultAsync(p => p.Id == productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product with ID '{ProductId}'", productId);
                return null;
            }
        }

        public async Task<Product?> GetProductByBarcodeAsync(string barcode)
        {
            try
            {
                return await _context.Products
                    .Include(p => p.Supplier)
                    .FirstOrDefaultAsync(p => p.Barcode == barcode && p.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product with barcode '{Barcode}'", barcode);
                return null;
            }
        }

        public async Task<List<Product>> SearchProductsAsync(string searchTerm)
        {
            try
            {
                searchTerm = searchTerm.ToLower();

                return await _context.Products
                    .Include(p => p.Supplier)
                    .Where(p => p.IsActive &&
                        (p.Name.ToLower().Contains(searchTerm) ||
                         p.ProductCode!.ToLower().Contains(searchTerm) ||
                         p.Barcode!.Contains(searchTerm) ||
                         p.Category!.ToLower().Contains(searchTerm) ||
                         p.Brand!.ToLower().Contains(searchTerm)))
                    .OrderBy(p => p.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products with term '{SearchTerm}'", searchTerm);
                return new List<Product>();
            }
        }

        public async Task<bool> CreateProductAsync(Product product)
        {
            try
            {
                // Check if product code already exists
                if (!string.IsNullOrEmpty(product.ProductCode))
                {
                    var codeExists = await _context.Products
                        .AnyAsync(p => p.ProductCode == product.ProductCode);

                    if (codeExists)
                    {
                        _logger.LogWarning("Product creation failed: Product code '{ProductCode}' already exists", product.ProductCode);
                        return false;
                    }
                }

                // Check if barcode already exists
                if (!string.IsNullOrEmpty(product.Barcode))
                {
                    var barcodeExists = await _context.Products
                        .AnyAsync(p => p.Barcode == product.Barcode);

                    if (barcodeExists)
                    {
                        _logger.LogWarning("Product creation failed: Barcode '{Barcode}' already exists", product.Barcode);
                        return false;
                    }
                }

                product.CreatedDate = DateTime.Now;
                product.IsActive = true;

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                // Create initial stock ledger entry if current stock > 0
                if (product.CurrentStock > 0)
                {
                    await AddStockLedgerEntryAsync(product.Id, "Opening Stock", null, null,
                        product.CurrentStock, product.PurchasePrice, 0, product.CurrentStock,
                        "Initial stock entry");
                }

                _logger.LogInformation("Product '{ProductName}' created successfully", product.Name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product '{ProductName}'", product.Name);
                return false;
            }
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            try
            {
                var existingProduct = await _context.Products.FindAsync(product.Id);
                if (existingProduct == null)
                {
                    _logger.LogWarning("Product update failed: Product with ID '{ProductId}' not found", product.Id);
                    return false;
                }

                // Check if product code is being changed and already exists
                if (!string.IsNullOrEmpty(product.ProductCode) && product.ProductCode != existingProduct.ProductCode)
                {
                    var codeExists = await _context.Products
                        .AnyAsync(p => p.ProductCode == product.ProductCode && p.Id != product.Id);

                    if (codeExists)
                    {
                        _logger.LogWarning("Product update failed: Product code '{ProductCode}' already exists", product.ProductCode);
                        return false;
                    }
                }

                // Check if barcode is being changed and already exists
                if (!string.IsNullOrEmpty(product.Barcode) && product.Barcode != existingProduct.Barcode)
                {
                    var barcodeExists = await _context.Products
                        .AnyAsync(p => p.Barcode == product.Barcode && p.Id != product.Id);

                    if (barcodeExists)
                    {
                        _logger.LogWarning("Product update failed: Barcode '{Barcode}' already exists", product.Barcode);
                        return false;
                    }
                }

                // Update fields
                existingProduct.Name = product.Name;
                existingProduct.ProductCode = product.ProductCode;
                existingProduct.Barcode = product.Barcode;
                existingProduct.Category = product.Category;
                existingProduct.Brand = product.Brand;
                existingProduct.Unit = product.Unit;
                existingProduct.PurchasePrice = product.PurchasePrice;
                existingProduct.SellingPrice = product.SellingPrice;
                existingProduct.MRP = product.MRP;
                existingProduct.GST = product.GST;
                existingProduct.MinimumStock = product.MinimumStock;
                existingProduct.MaximumStock = product.MaximumStock;
                existingProduct.HSNCode = product.HSNCode;
                existingProduct.Description = product.Description;
                existingProduct.ImageUrl = product.ImageUrl;
                existingProduct.SupplierId = product.SupplierId;
                existingProduct.ExpiryDate = product.ExpiryDate;
                existingProduct.LastUpdated = DateTime.Now;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Product '{ProductName}' updated successfully", product.Name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product '{ProductName}'", product.Name);
                return false;
            }
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.BillItems)
                    .Include(p => p.PurchaseItems)
                    .FirstOrDefaultAsync(p => p.Id == productId);

                if (product == null)
                {
                    _logger.LogWarning("Product deletion failed: Product with ID '{ProductId}' not found", productId);
                    return false;
                }

                // Check if product has been used in any bills or purchases
                if (product.BillItems.Any() || product.PurchaseItems.Any())
                {
                    // Soft delete - mark as inactive
                    product.IsActive = false;
                    product.LastUpdated = DateTime.Now;
                }
                else
                {
                    // Hard delete if not used
                    _context.Products.Remove(product);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Product '{ProductName}' deleted successfully", product.Name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with ID '{ProductId}'", productId);
                return false;
            }
        }

        public async Task<bool> UpdateStockAsync(int productId, decimal newStock, string reason, string? referenceNo = null, int? referenceId = null)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    _logger.LogWarning("Stock update failed: Product with ID '{ProductId}' not found", productId);
                    return false;
                }

                var oldStock = product.CurrentStock;
                var quantity = newStock - oldStock;

                product.CurrentStock = newStock;
                product.LastUpdated = DateTime.Now;

                // Add stock ledger entry
                await AddStockLedgerEntryAsync(productId, "Stock Adjustment", referenceNo, referenceId,
                    quantity, product.PurchasePrice, oldStock, newStock, reason);

                await _context.SaveChangesAsync();

                _logger.LogInformation("Stock updated for product '{ProductId}' from {OldStock} to {NewStock}",
                    productId, oldStock, newStock);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock for product '{ProductId}'", productId);
                return false;
            }
        }

        public async Task<List<Product>> GetLowStockProductsAsync()
        {
            try
            {
                return await _context.Products
                    .Include(p => p.Supplier)
                    .Where(p => p.IsActive && p.CurrentStock <= p.MinimumStock)
                    .OrderBy(p => p.CurrentStock)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting low stock products");
                return new List<Product>();
            }
        }

        public async Task<List<Product>> GetExpiringProductsAsync(int daysAhead = 30)
        {
            try
            {
                var cutoffDate = DateTime.Now.AddDays(daysAhead);

                return await _context.Products
                    .Include(p => p.Supplier)
                    .Where(p => p.IsActive &&
                           p.ExpiryDate.HasValue &&
                           p.ExpiryDate.Value <= cutoffDate &&
                           p.CurrentStock > 0)
                    .OrderBy(p => p.ExpiryDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting expiring products");
                return new List<Product>();
            }
        }

        public async Task<List<StockLedger>> GetStockLedgerAsync(int productId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var query = _context.StockLedgers
                    .Include(s => s.CreatedBy)
                    .Where(s => s.ProductId == productId);

                if (fromDate.HasValue)
                    query = query.Where(s => s.TransactionDate >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(s => s.TransactionDate <= toDate.Value);

                return await query
                    .OrderByDescending(s => s.TransactionDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock ledger for product '{ProductId}'", productId);
                return new List<StockLedger>();
            }
        }

        public async Task<bool> AddStockLedgerEntryAsync(int productId, string transactionType, string? referenceNo, int? referenceId, decimal quantity, decimal rate, decimal stockBefore, decimal stockAfter, string? notes = null)
        {
            try
            {
                var stockLedger = new StockLedger
                {
                    ProductId = productId,
                    TransactionType = transactionType,
                    ReferenceNo = referenceNo,
                    ReferenceId = referenceId,
                    Quantity = quantity,
                    Rate = rate,
                    StockBefore = stockBefore,
                    StockAfter = stockAfter,
                    TransactionDate = DateTime.Now,
                    Notes = notes,
                    CreatedByUserId = _sessionService.CurrentUserId ?? 1
                };

                _context.StockLedgers.Add(stockLedger);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding stock ledger entry for product '{ProductId}'", productId);
                return false;
            }
        }

        public async Task<List<string>> GetCategoriesAsync()
        {
            try
            {
                return await _context.Products
                    .Where(p => p.IsActive && !string.IsNullOrEmpty(p.Category))
                    .Select(p => p.Category!)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product categories");
                return new List<string>();
            }
        }

        public async Task<List<string>> GetBrandsAsync()
        {
            try
            {
                return await _context.Products
                    .Where(p => p.IsActive && !string.IsNullOrEmpty(p.Brand))
                    .Select(p => p.Brand!)
                    .Distinct()
                    .OrderBy(b => b)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product brands");
                return new List<string>();
            }
        }
    }

    public class BillingService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<BillingService> _logger;
        private readonly SessionService _sessionService;
        private readonly ProductService _productService;

        public BillingService(AppDbContext context, ILogger<BillingService> logger, SessionService sessionService, ProductService productService)
        {
            _context = context;
            _logger = logger;
            _sessionService = sessionService;
            _productService = productService;
        }

        public async Task<List<Bill>> GetAllBillsAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var query = _context.Bills
                    .Include(b => b.Customer)
                    .Include(b => b.CreatedBy)
                    .Where(b => b.IsActive);

                if (fromDate.HasValue)
                    query = query.Where(b => b.BillDate >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(b => b.BillDate <= toDate.Value);

                return await query
                    .OrderByDescending(b => b.BillDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all bills");
                return new List<Bill>();
            }
        }

        public async Task<Bill?> GetBillByIdAsync(int billId)
        {
            try
            {
                return await _context.Bills
                    .Include(b => b.Customer)
                    .Include(b => b.CreatedBy)
                    .Include(b => b.Items)
                        .ThenInclude(i => i.Product)
                    .Include(b => b.Payments)
                    .FirstOrDefaultAsync(b => b.Id == billId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bill with ID '{BillId}'", billId);
                return null;
            }
        }

        public async Task<Bill?> GetBillByBillNoAsync(string billNo)
        {
            try
            {
                return await _context.Bills
                    .Include(b => b.Customer)
                    .Include(b => b.CreatedBy)
                    .Include(b => b.Items)
                        .ThenInclude(i => i.Product)
                    .FirstOrDefaultAsync(b => b.BillNo == billNo && b.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bill with bill number '{BillNo}'", billNo);
                return null;
            }
        }

        public async Task<string> GenerateBillNumberAsync()
        {
            try
            {
                var today = DateTime.Now;
                var prefix = $"INV{today:yyyyMM}";

                var lastBill = await _context.Bills
                    .Where(b => b.BillNo.StartsWith(prefix))
                    .OrderByDescending(b => b.BillNo)
                    .FirstOrDefaultAsync();

                if (lastBill == null)
                {
                    return $"{prefix}0001";
                }

                // Extract the sequence number
                var lastNumber = lastBill.BillNo.Substring(prefix.Length);
                if (int.TryParse(lastNumber, out int sequence))
                {
                    return $"{prefix}{(sequence + 1):D4}";
                }

                return $"{prefix}0001";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating bill number");
                return $"INV{DateTime.Now:yyyyMMddHHmmss}";
            }
        }

        public async Task<int?> CreateBillAsync(Bill bill, List<BillItem> items)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Generate bill number if not provided
                if (string.IsNullOrEmpty(bill.BillNo))
                {
                    bill.BillNo = await GenerateBillNumberAsync();
                }

                bill.CreatedByUserId = _sessionService.CurrentUserId ?? 1;
                bill.CreatedDate = DateTime.Now;
                bill.IsActive = true;

                // Calculate totals
                CalculateBillTotals(bill, items);

                _context.Bills.Add(bill);
                await _context.SaveChangesAsync();

                // Add bill items and update stock
                foreach (var item in items)
                {
                    item.BillId = bill.Id;
                    _context.BillItems.Add(item);

                    // Update product stock
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        var oldStock = product.CurrentStock;
                        product.CurrentStock -= item.Quantity;
                        product.LastUpdated = DateTime.Now;

                        // Add stock ledger entry
                        await _productService.AddStockLedgerEntryAsync(item.ProductId, "Sale",
                            bill.BillNo, bill.Id, -item.Quantity, item.Rate,
                            oldStock, product.CurrentStock, $"Sale - Bill {bill.BillNo}");
                    }
                }

                // Update customer outstanding if customer exists
                if (bill.CustomerId.HasValue && bill.BalanceAmount > 0)
                {
                    var customer = await _context.Customers.FindAsync(bill.CustomerId.Value);
                    if (customer != null)
                    {
                        customer.OutstandingAmount += bill.BalanceAmount;
                        customer.LastUpdated = DateTime.Now;
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Bill '{BillNo}' created successfully with total amount {TotalAmount}",
                    bill.BillNo, bill.TotalAmount);

                return bill.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating bill");
                return null;
            }
        }

        public async Task<bool> UpdateBillAsync(int billId, Bill updatedBill, List<BillItem> updatedItems)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingBill = await _context.Bills
                    .Include(b => b.Items)
                    .FirstOrDefaultAsync(b => b.Id == billId);

                if (existingBill == null)
                {
                    _logger.LogWarning("Bill update failed: Bill with ID '{BillId}' not found", billId);
                    return false;
                }

                // Restore stock for existing items
                foreach (var existingItem in existingBill.Items)
                {
                    var product = await _context.Products.FindAsync(existingItem.ProductId);
                    if (product != null)
                    {
                        var oldStock = product.CurrentStock;
                        product.CurrentStock += existingItem.Quantity;

                        await _productService.AddStockLedgerEntryAsync(existingItem.ProductId, "Sale Return",
                            existingBill.BillNo, existingBill.Id, existingItem.Quantity, existingItem.Rate,
                            oldStock, product.CurrentStock, $"Stock restoration - Bill {existingBill.BillNo} update");
                    }
                }

                // Remove existing items
                _context.BillItems.RemoveRange(existingBill.Items);

                // Update bill details
                existingBill.CustomerId = updatedBill.CustomerId;
                existingBill.CustomerName = updatedBill.CustomerName;
                existingBill.CustomerPhone = updatedBill.CustomerPhone;
                existingBill.CustomerAddress = updatedBill.CustomerAddress;
                existingBill.PaymentMode = updatedBill.PaymentMode;
                existingBill.PaymentReference = updatedBill.PaymentReference;
                existingBill.PaymentStatus = updatedBill.PaymentStatus;
                existingBill.SalesPerson = updatedBill.SalesPerson;
                existingBill.Notes = updatedBill.Notes;
                existingBill.LastUpdated = DateTime.Now;

                // Calculate new totals
                CalculateBillTotals(existingBill, updatedItems);

                // Add updated items and update stock
                foreach (var item in updatedItems)
                {
                    item.BillId = billId;
                    _context.BillItems.Add(item);

                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        var oldStock = product.CurrentStock;
                        product.CurrentStock -= item.Quantity;
                        product.LastUpdated = DateTime.Now;

                        await _productService.AddStockLedgerEntryAsync(item.ProductId, "Sale",
                            existingBill.BillNo, existingBill.Id, -item.Quantity, item.Rate,
                            oldStock, product.CurrentStock, $"Sale - Bill {existingBill.BillNo} (Updated)");
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Bill '{BillNo}' updated successfully", existingBill.BillNo);
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error updating bill with ID '{BillId}'", billId);
                return false;
            }
        }

        public async Task<bool> DeleteBillAsync(int billId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var bill = await _context.Bills
                    .Include(b => b.Items)
                    .Include(b => b.Customer)
                    .FirstOrDefaultAsync(b => b.Id == billId);

                if (bill == null)
                {
                    _logger.LogWarning("Bill deletion failed: Bill with ID '{BillId}' not found", billId);
                    return false;
                }

                // Restore stock for all items
                foreach (var item in bill.Items)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        var oldStock = product.CurrentStock;
                        product.CurrentStock += item.Quantity;
                        product.LastUpdated = DateTime.Now;

                        await _productService.AddStockLedgerEntryAsync(item.ProductId, "Sale Cancelled",
                            bill.BillNo, bill.Id, item.Quantity, item.Rate,
                            oldStock, product.CurrentStock, $"Stock restoration - Bill {bill.BillNo} deleted");
                    }
                }

                // Update customer outstanding if customer exists
                if (bill.Customer != null && bill.BalanceAmount > 0)
                {
                    bill.Customer.OutstandingAmount -= bill.BalanceAmount;
                    if (bill.Customer.OutstandingAmount < 0)
                        bill.Customer.OutstandingAmount = 0;
                    bill.Customer.LastUpdated = DateTime.Now;
                }

                // Soft delete
                bill.IsActive = false;
                bill.LastUpdated = DateTime.Now;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Bill '{BillNo}' deleted successfully", bill.BillNo);
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error deleting bill with ID '{BillId}'", billId);
                return false;
            }
        }

        public async Task<List<Bill>> SearchBillsAsync(string searchTerm, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                searchTerm = searchTerm.ToLower();

                var query = _context.Bills
                    .Include(b => b.Customer)
                    .Include(b => b.CreatedBy)
                    .Where(b => b.IsActive &&
                        (b.BillNo.ToLower().Contains(searchTerm) ||
                         b.CustomerName!.ToLower().Contains(searchTerm) ||
                         b.CustomerPhone!.Contains(searchTerm)));

                if (fromDate.HasValue)
                    query = query.Where(b => b.BillDate >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(b => b.BillDate <= toDate.Value);

                return await query
                    .OrderByDescending(b => b.BillDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching bills with term '{SearchTerm}'", searchTerm);
                return new List<Bill>();
            }
        }

        private void CalculateBillTotals(Bill bill, List<BillItem> items)
        {
            bill.SubTotal = items.Sum(i => i.Total);

            // Calculate GST
            bill.GSTAmount = items.Sum(i => i.GSTAmount);
            bill.CGSTAmount = items.Sum(i => i.CGSTAmount);
            bill.SGSTAmount = items.Sum(i => i.SGSTAmount);
            bill.IGSTAmount = items.Sum(i => i.IGSTAmount);

            // Calculate total after discount
            var totalAfterDiscount = bill.SubTotal - bill.DiscountAmount;
            bill.TotalAmount = totalAfterDiscount + bill.GSTAmount;

            // Calculate balance
            bill.BalanceAmount = bill.TotalAmount - bill.PaidAmount;
        }

        public async Task<Dictionary<string, object>> GetSalesSummaryAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var bills = await _context.Bills
                    .Where(b => b.IsActive &&
                           b.BillDate >= fromDate &&
                           b.BillDate <= toDate)
                    .ToListAsync();

                return new Dictionary<string, object>
                {
                    {"TotalBills", bills.Count},
                    {"TotalSales", bills.Sum(b => b.TotalAmount)},
                    {"TotalTax", bills.Sum(b => b.GSTAmount)},
                    {"TotalDiscount", bills.Sum(b => b.DiscountAmount)},
                    {"CashSales", bills.Where(b => b.PaymentMode == "Cash").Sum(b => b.TotalAmount)},
                    {"CreditSales", bills.Where(b => b.PaymentMode == "Credit").Sum(b => b.TotalAmount)},
                    {"CardSales", bills.Where(b => b.PaymentMode == "Card").Sum(b => b.TotalAmount)},
                    {"UpiSales", bills.Where(b => b.PaymentMode == "UPI").Sum(b => b.TotalAmount)}
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sales summary");
                return new Dictionary<string, object>();
            }
        }
    }
}
