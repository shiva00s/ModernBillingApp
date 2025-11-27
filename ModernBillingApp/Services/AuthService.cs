using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using ModernBillingApp.Data;
using ModernBillingApp.Models;

// This service replaces your old "checklogin" logic
public class AuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    // This is the new, SECURE login method
    public async Task<User> Login(string username, string password)
    {
        // 1. Find the user (securely, no SQL injection)
        var user = await _context.Users
            .Include(u => u.Role) // Also get the user's role
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user == null)
        {
            return null; // User not found
        }

        // 2. Verify the hashed password
        if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
        {
            return null; // Invalid password
        }

        return user; // Login successful
    }

    // This is the helper function to check the password
    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512(passwordSalt))
        {
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
    }

    // This is the function we will use for frmAdmin to create new users
    public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
}