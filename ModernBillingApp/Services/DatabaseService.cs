using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using ModernBillingApp.Data;
using ModernBillingApp.Models;

// This service replaces the logic from frmDatabaseBackUp.cs
public class DatabaseService
{
    private readonly AppDbContext _context;

    public DatabaseService(AppDbContext context)
    {
        _context = context;
    }

    // This replaces your "button5_Click" (BACKUP)
    public async Task<string> BackupDatabase(string backupPath)
    {
        try
        {
            // Get the database name from the connection
            var dbName = _context.Database.GetDbConnection().Database;

            // Create a full file path
            // e.g., C:\Backups\ModernBillingApp-20251108.bak
            string fileName = $"{dbName}-{DateTime.Now:yyyyMMdd}.bak";
            string fullPath = Path.Combine(backupPath, fileName);

            // This is the modern, safe way to run a raw SQL command
            string sqlCommand = $"BACKUP DATABASE [{dbName}] TO DISK = @path";

            await _context.Database.ExecuteSqlRawAsync(sqlCommand, new SqlParameter("@path", fullPath));

            return $"Backup successful! Saved to: {fullPath}";
        }
        catch (Exception ex)
        {
            // This will catch errors like "Path not found" or "Permission denied"
            return $"Error: {ex.Message}";
        }
    }
}