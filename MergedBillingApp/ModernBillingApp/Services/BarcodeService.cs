using Microsoft.EntityFrameworkCore;
using ModernBillingApp.Data;
using ModernBillingApp.Models;

namespace ModernBillingApp.Services
{
    // Service for barcode scanning and lookup
    public class BarcodeService
    {
        private readonly AppDbContext _context;

        public BarcodeService(AppDbContext context)
        {
            _context = context;
        }

        // Find product by barcode
        public async Task<Product?> FindProductByBarcode(string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode))
                return null;

            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Barcode == barcode);
        }

        // Find product by product ID (PID)
        public async Task<Product?> FindProductByPID(string pid)
        {
            if (string.IsNullOrWhiteSpace(pid))
                return null;

            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.PID == pid);
        }

        // Search products by name (for autocomplete)
        public async Task<List<Product>> SearchProducts(string searchTerm, int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<Product>();

            return await _context.Products
                .Include(p => p.Category)
                .Where(p => 
                    (p.PName != null && p.PName.Contains(searchTerm)) ||
                    (p.PID != null && p.PID.Contains(searchTerm)) ||
                    (p.Barcode != null && p.Barcode.Contains(searchTerm))
                )
                .Where(p => p.CurrentStock > 0) // Only show products in stock
                .Take(limit)
                .ToListAsync();
        }

        // Validate barcode format (EAN-13, UPC, etc.)
        public bool ValidateBarcode(string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode))
                return false;

            // EAN-13: 13 digits
            // UPC: 12 digits
            // EAN-8: 8 digits
            return System.Text.RegularExpressions.Regex.IsMatch(barcode, @"^\d{8,13}$");
        }
    }
}

