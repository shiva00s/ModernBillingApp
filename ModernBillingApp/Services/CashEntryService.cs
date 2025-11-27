using Microsoft.EntityFrameworkCore;
using ModernBillingApp.Data;
using ModernBillingApp.Models;

namespace ModernBillingApp.Services
{
    public class CashEntryService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CashEntryService> _logger;

        public CashEntryService(AppDbContext context, ILogger<CashEntryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<CashEntry>> GetCashEntries()
        {
            try
            {
                return await _context.CashEntries
                    .OrderByDescending(c => c.Date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cash entries");
                return new List<CashEntry>();
            }
        }

        public async Task<CashEntry?> GetCashEntryById(int id)
        {
            try
            {
                return await _context.CashEntries.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cash entry with ID {Id}", id);
                return null;
            }
        }

        public async Task<CashEntry> CreateCashEntry(CashEntry entry)
        {
            try
            {
                _context.CashEntries.Add(entry);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Created cash entry: {EntryName}", entry.CEName);
                return entry;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating cash entry");
                throw;
            }
        }

        public async Task<CashEntry> UpdateCashEntry(CashEntry entry)
        {
            try
            {
                _context.CashEntries.Update(entry);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated cash entry: {EntryName}", entry.CEName);
                return entry;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cash entry with ID {Id}", entry.Id);
                throw;
            }
        }

        public async Task DeleteCashEntry(int id)
        {
            try
            {
                var entry = await _context.CashEntries.FindAsync(id);
                if (entry != null)
                {
                    _context.CashEntries.Remove(entry);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Deleted cash entry with ID {Id}", id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting cash entry with ID {Id}", id);
                throw;
            }
        }

        public async Task<List<CashEntry>> GetCashEntriesByType(string type)
        {
            try
            {
                return await _context.CashEntries
                    .Where(c => c.CEType == type)
                    .OrderByDescending(c => c.Date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cash entries by type {Type}", type);
                return new List<CashEntry>();
            }
        }

        public async Task<List<CashEntry>> GetCashEntriesByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _context.CashEntries
                    .Where(c => c.Date >= startDate && c.Date <= endDate)
                    .OrderByDescending(c => c.Date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cash entries by date range");
                return new List<CashEntry>();
            }
        }

        public async Task<decimal> GetTotalIncome(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var query = _context.CashEntries.Where(c => c.CEType == "Income");

                if (startDate.HasValue)
                    query = query.Where(c => c.Date >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(c => c.Date <= endDate.Value);

                return await query.SumAsync(c => c.Amount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total income");
                return 0;
            }
        }

        public async Task<decimal> GetTotalExpense(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var query = _context.CashEntries.Where(c => c.CEType == "Expense");

                if (startDate.HasValue)
                    query = query.Where(c => c.Date >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(c => c.Date <= endDate.Value);

                return await query.SumAsync(c => c.Amount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total expense");
                return 0;
            }
        }

        public async Task<decimal> GetNetCashFlow(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var income = await GetTotalIncome(startDate, endDate);
                var expense = await GetTotalExpense(startDate, endDate);
                return income - expense;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating net cash flow");
                return 0;
            }
        }

        public async Task<List<CashEntry>> SearchCashEntries(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return await GetCashEntries();

                searchTerm = searchTerm.ToLower();

                return await _context.CashEntries
                    .Where(c =>
                        c.CEName!.ToLower().Contains(searchTerm) ||
                        c.CEType!.ToLower().Contains(searchTerm) ||
                        (c.Remark != null && c.Remark.ToLower().Contains(searchTerm)))
                    .OrderByDescending(c => c.Date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching cash entries with term '{SearchTerm}'", searchTerm);
                return new List<CashEntry>();
            }
        }
    }
}
