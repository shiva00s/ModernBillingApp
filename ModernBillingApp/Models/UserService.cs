using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using ModernBillingApp.Data;
using ModernBillingApp.Models;

// This service replaces all the query logic from frmAdmin.cs
public class UserService
{
    private readonly AppDbContext _context;
    private readonly AuthService _authService;

    public UserService(AppDbContext context, AuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<List<User>> GetUsers()
    {
        // Securely gets all users and their roles
        return await _context.Users.Include(u => u.Role).ToListAsync();
    }

    public async Task<User> GetUserById(int id)
    {
        return await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<List<UserRole>> GetRoles()
    {
        // Gets the list of roles to show in a dropdown
        return await _context.UserRoles.ToListAsync();
    }

    // This REPLACES your "Save" button (button6_Click in frmAdmin)
    public async Task<User> CreateUser(string username, string password, string email, int roleId)
    {
        _authService.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            UserRoleId = roleId
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    // This REPLACES your "Update" button (button7_Click in frmAdmin)
    public async Task UpdateUser(int userId, string username, string email, int roleId, string newPassword = null)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return;

        user.Username = username;
        user.Email = email;
        user.UserRoleId = roleId;

        // Only update password if a new one is provided
        if (!string.IsNullOrEmpty(newPassword))
        {
            _authService.CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
        }

        await _context.SaveChangesAsync();
    }

    // This REPLACES your "Delete" button (button4_Click in frmAdmin)
    public async Task DeleteUser(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}