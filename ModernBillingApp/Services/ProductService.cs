using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using ModernBillingApp.Data;
using ModernBillingApp.Models;

namespace ModernBillingApp.Services
{
    // This service replaces all the query logic from frmStockEntrySpaProduct.cs
    public class ProductService
    {
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    // --- Product Catalog Methods ---

    public async Task<List<Product>> GetProducts()
    {
        return await _context.Products.Include(p => p.Category).ToListAsync();
    }

    public async Task<Product> GetProductById(int id)
    {
        return await _context.Products.Include(p => p.Category)
                                    .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task CreateProduct(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateProduct(Product product)
    {
        _context.Entry(product).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteProduct(int productId)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Category>> GetCategories()
    {
        return await _context.Categories.ToListAsync();
    }

    public async Task<List<Supplier>> GetSuppliers()
    {
        return await _context.Suppliers.ToListAsync();
    }

    // --- Stock Management Methods ---

    // This REPLACES your "Save" button logic for adding stock
    public async Task AddStock(StockLedger entry)
    {
        // 1. Add the new log entry
        entry.Date = DateTime.Now;
        _context.StockLedgers.Add(entry);

        // 2. Update the master product's stock level
        var product = await _context.Products.FindAsync(entry.ProductId);
        if (product != null)
        {
            product.CurrentStock += entry.QtyAdded;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<StockLedger>> GetStockHistory(int productId)
    {
        return await _context.StockLedgers
                             .Where(s => s.ProductId == productId)
                             .Include(s => s.Supplier)
                             .OrderByDescending(s => s.Date)
                             .ToListAsync();
    }

    // --- PASTE THIS CODE ---

    // This REPLACES your "Save" button logic from frmStockReturnProduct.cs
    public async Task ReturnStock(StockReturn returnEntry, int productIdToUpdate)
    {
        // 1. Add the "StockReturn" log entry
        returnEntry.Date = DateTime.Now;
        _context.StockReturns.Add(returnEntry);

        // 2. Add a corresponding NEGATIVE entry to the StockLedger
        var ledgerEntry = new StockLedger
        {
            Date = DateTime.Now,
            BatchNo = returnEntry.BatchNo,
            PurNo = returnEntry.PurNo,
            QtyAdded = -returnEntry.TQty, // Negative quantity
            PPrice = returnEntry.PPrice,
            ProductId = productIdToUpdate,
            SupplierId = null // Or find supplier ID if needed
        };
        _context.StockLedgers.Add(ledgerEntry);

        // 3. Update the master product's stock level
        var product = await _context.Products.FindAsync(productIdToUpdate);
        if (product != null)
        {
            product.CurrentStock -= returnEntry.TQty; // Subtract stock
        }

        await _context.SaveChangesAsync();
    }

    // --- END OF PASTE ---

    // Real-time stock validation
    public async Task<bool> ValidateStockAvailability(int productId, double requestedQty)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null) return false;
        return product.CurrentStock >= requestedQty;
    }

    public async Task<List<Product>> GetLowStockProducts(int threshold = 10)
    {
        return await _context.Products
            .Where(p => p.CurrentStock <= threshold)
                .OrderBy(p => p.CurrentStock)
                .ToListAsync();
        }
        }
    }
