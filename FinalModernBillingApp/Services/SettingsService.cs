using Microsoft.EntityFrameworkCore;
using ModernBillingApp.Data;
using ModernBillingApp.Models;

namespace ModernBillingApp.Services
{
    public class SettingsService
    {
        private readonly AppDbContext _context;

        public SettingsService(AppDbContext context)
        {
            _context = context;
        }

    // This gets the one and only settings row
    public async Task<ShopSettings> GetSettings()
    {
        // We use FirstOrDefault() to get the first row, or null if it's empty
        var settings = await _context.ShopSettings.FirstOrDefaultAsync();
        if (settings == null)
        {
            // If no settings exist, return a new, empty one
            return new ShopSettings();
        }
        return settings;
    }

    // This saves or updates the settings
    public async Task SaveSettings(ShopSettings settings)
    {
        if (settings.Id == 0)
        {
            // It's a new entry
            _context.ShopSettings.Add(settings);
        }
        else
        {
            // It's an existing entry
            _context.Entry(settings).State = EntityState.Modified;
        }
        await _context.SaveChangesAsync();
    }
    }
}
