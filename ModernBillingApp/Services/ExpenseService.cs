using Microsoft.EntityFrameworkCore;
using ModernBillingApp.Data;
using ModernBillingApp.Models;

// This service replaces all the query logic from frmExpenses.cs
public class ExpenseService
{
    private readonly AppDbContext _context;

    public ExpenseService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CashEntry>> GetCashEntries(DateTime? fromDate = null, DateTime? toDate = null, string? type = null)
    {
        var query = _context.CashEntries.AsQueryable();

        if (fromDate.HasValue)
            query = query.Where(c => c.Date >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(c => c.Date <= toDate.Value);

        if (!string.IsNullOrEmpty(type))
            query = query.Where(c => c.CEType == type);

        return await query.OrderByDescending(c => c.Date).ToListAsync();
    }

    // This replaces your "Save" button (button13_Click)
    public async Task CreateCashEntry(CashEntry entry)
    {
        _context.CashEntries.Add(entry);
        await _context.SaveChangesAsync();
    }

    // This replaces your "Update" button (button14_Click)
    public async Task UpdateCashEntry(CashEntry entry)
    {
        _context.Entry(entry).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    // Overload for updating with individual parameters
    public async Task UpdateCashEntry(int id, string name, string type, DateTime date, decimal amount, string? remark = null)
    {
        var entry = await _context.CashEntries.FindAsync(id);
        if (entry != null)
        {
            entry.CEName = name;
            entry.CEType = type;
            entry.Date = date;
            entry.Amount = amount;
            entry.Remark = remark;
            await _context.SaveChangesAsync();
        }
    }

    // Overload for creating with individual parameters
    public async Task AddCashEntry(string name, string type, DateTime date, decimal amount, string? remark = null)
    {
        var entry = new CashEntry
        {
            CEName = name,
            CEType = type,
            Date = date,
            Amount = amount,
            Remark = remark
        };
        _context.CashEntries.Add(entry);
        await _context.SaveChangesAsync();
    }

    // This replaces your "Delete" button (button20_Click)
    public async Task DeleteCashEntry(int entryId)
    {
        var entry = await _context.CashEntries.FindAsync(entryId);
        if (entry != null)
        {
            _context.CashEntries.Remove(entry);
            await _context.SaveChangesAsync();
        }
    }
}