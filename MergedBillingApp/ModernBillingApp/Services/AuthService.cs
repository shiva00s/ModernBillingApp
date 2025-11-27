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

    public async Task<User> CreateUserAsync(string username, string email, string password, int roleId, string fullName, string phone)
    {
        CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

        var userRole = await _context.UserRoles.FindAsync(roleId);
        if (userRole == null)
        {
            throw new Exception($"Role with ID {roleId} not found.");
        }

        var newUser = new User
        {
            Username = username,
            Email = email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            UserRoleId = roleId,
            Role = userRole,
            // Add other properties if available in your User model
            // FullName = fullName,
            // PhoneNumber = phone
        };

        await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();
        return newUser;
    }
}
