using Microsoft.EntityFrameworkCore;
using ModernBillingApp.Data;
using ModernBillingApp.Models;

// This service replaces all the query logic from frmSupplierEntry.cs
public class SupplierService
{
    private readonly AppDbContext _context;

    public SupplierService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Supplier>> GetSuppliers()
    {
        return await _context.Suppliers.ToListAsync();
    }

    // This replaces your "Save" button (button6_Click)
    public async Task<Supplier> CreateSupplier(Supplier supplier)
    {
        supplier.VDate = DateTime.Now; // Set the creation date
        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();
        return supplier;
    }

    // This replaces your "Update" button (button7_Click)
    public async Task UpdateSupplier(Supplier supplier)
    {
        _context.Entry(supplier).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    // This replaces your "Delete" button (button4_Click)
    public async Task DeleteSupplier(int supplierId)
    {
        var supplier = await _context.Suppliers.FindAsync(supplierId);
        if (supplier != null)
        {
            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();
        }
    }
}