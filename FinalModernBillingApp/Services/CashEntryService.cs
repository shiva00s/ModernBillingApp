using Microsoft.EntityFrameworkCore;
using ModernBillingApp.Data;
using ModernBillingApp.Models;

namespace ModernBillingApp.Services
{
    public class CashEntryService
    {
        private readonly AppDbContext _context;
        private readonly SessionService _sessionService;

        public CashEntryService(AppDbContext context, SessionService sessionService)
        {
            _context = context;
            _sessionService = sessionService;
        }

        public async Task<List<CashEntry>> GetAllCashEntriesAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var query = _context.CashEntries
                    .Include(c => c.Customer)
                    .Include(c => c.Supplier)
                    .Include(c => c.CreatedBy)
                    .Where(c => c.IsActive);

                if (fromDate.HasValue)
                    query = query.Where(c => c.EntryDate >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(c => c.EntryDate <= toDate.Value);

                return await query
                    .OrderByDescending(c => c.EntryDate)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<CashEntry>();
            }
        }

        public async Task<CashEntry?> GetCashEntryByIdAsync(int entryId)
        {
            try
            {
                return await _context.CashEntries
                    .Include(c => c.Customer)
                    .Include(c => c.Supplier)
                    .Include(c => c.CreatedBy)
                    .FirstOrDefaultAsync(c => c.Id == entryId);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> CreateCashEntryAsync(CashEntry cashEntry)
        {
            try
            {
                cashEntry.CreatedByUserId = _sessionService.CurrentUserId ?? 1;
                cashEntry.CreatedDate = DateTime.Now;
                cashEntry.IsActive = true;

                _context.CashEntries.Add(cashEntry);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateCashEntryAsync(CashEntry cashEntry)
        {
            try
            {
                var existingEntry = await _context.CashEntries.FindAsync(cashEntry.Id);
                if (existingEntry == null)
                {
                    return false;
                }

                existingEntry.EntryName = cashEntry.EntryName;
                existingEntry.EntryType = cashEntry.EntryType;
                existingEntry.Category = cashEntry.Category;
                existingEntry.Amount = cashEntry.Amount;
                existingEntry.EntryDate = cashEntry.EntryDate;
                existingEntry.Description = cashEntry.Description;
                existingEntry.PaymentMode = cashEntry.PaymentMode;
                existingEntry.TransactionReference = cashEntry.TransactionReference;
                existingEntry.CustomerId = cashEntry.CustomerId;
                existingEntry.SupplierId = cashEntry.SupplierId;
                existingEntry.ReceivedFrom = cashEntry.ReceivedFrom;
                existingEntry.PaidTo = cashEntry.PaidTo;
                existingEntry.Notes = cashEntry.Notes;
                existingEntry.LastUpdated = DateTime.Now;

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteCashEntryAsync(int entryId)
        {
            try
            {
                var cashEntry = await _context.CashEntries.FindAsync(entryId);
                if (cashEntry == null)
                {
                    return false;
                }

                cashEntry.IsActive = false;
                cashEntry.LastUpdated = DateTime.Now;

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<CashEntry>> GetCashEntriesByTypeAsync(string entryType, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var query = _context.CashEntries
                    .Include(c => c.Customer)
                    .Include(c => c.Supplier)
                    .Include(c => c.CreatedBy)
                    .Where(c => c.IsActive && c.EntryType == entryType);

                if (fromDate.HasValue)
                    query = query.Where(c => c.EntryDate >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(c => c.EntryDate <= toDate.Value);

                return await query
                    .OrderByDescending(c => c.EntryDate)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<CashEntry>();
            }
        }

        public async Task<Dictionary<string, decimal>> GetCashSummaryAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var entries = await _context.CashEntries
                    .Where(c => c.IsActive &&
                           c.EntryDate >= fromDate &&
                           c.EntryDate <= toDate)
                    .ToListAsync();

                var income = entries.Where(c => c.EntryType == "Income").Sum(c => c.Amount);
                var expense = entries.Where(c => c.EntryType == "Expense").Sum(c => c.Amount);

                return new Dictionary<string, decimal>
                {
                    {"TotalIncome", income},
                    {"TotalExpense", expense},
                    {"NetCashFlow", income - expense},
                    {"CashEntries", entries.Count}
                };
            }
            catch (Exception)
            {
                return new Dictionary<string, decimal>
                {
                    {"TotalIncome", 0},
                    {"TotalExpense", 0},
                    {"NetCashFlow", 0},
                    {"CashEntries", 0}
                };
            }
        }

        public async Task<List<string>> GetCategoriesAsync()
        {
            try
            {
                return await _context.CashEntries
                    .Where(c => c.IsActive && !string.IsNullOrEmpty(c.Category))
                    .Select(c => c.Category!)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public async Task<List<CashEntry>> SearchCashEntriesAsync(string searchTerm, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                searchTerm = searchTerm.ToLower();

                var query = _context.CashEntries
                    .Include(c => c.Customer)
                    .Include(c => c.Supplier)
                    .Include(c => c.CreatedBy)
                    .Where(c => c.IsActive &&
                        (c.EntryName.ToLower().Contains(searchTerm) ||
                         (c.Description != null && c.Description.ToLower().Contains(searchTerm)) ||
                         (c.Category != null && c.Category.ToLower().Contains(searchTerm)) ||
                         (c.ReceivedFrom != null && c.ReceivedFrom.ToLower().Contains(searchTerm)) ||
                         (c.PaidTo != null && c.PaidTo.ToLower().Contains(searchTerm))));

                if (fromDate.HasValue)
                    query = query.Where(c => c.EntryDate >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(c => c.EntryDate <= toDate.Value);

                return await query
                    .OrderByDescending(c => c.EntryDate)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<CashEntry>();
            }
        }
    }
}
