using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using ModernBillingApp.Data;
using ModernBillingApp.Models;

namespace ModernBillingApp.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        // Main login method using BCrypt
        public async Task<User?> Login(string username, string password)
        {
            // Find the user (securely, no SQL injection)
            var user = await _context.Users
                .Include(u => u.Role) // Also get the user's role
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return null; // User not found
            }

            // For backward compatibility: check if old format (byte array) or new format (string)
            if (user.PasswordHash != null && user.PasswordHash.Length > 0)
            {
                // Old format: byte[] with HMACSHA512
                if (user.PasswordSalt != null && user.PasswordSalt.Length > 0)
                {
                    if (VerifyPasswordHashLegacy(password, user.PasswordHash, user.PasswordSalt))
                    {
                        // Migrate to BCrypt
                        await MigratePasswordToBCrypt(user, password);
                        return user;
                    }
                    return null;
                }
                else
                {
                    // New format: string with BCrypt
                    var passwordHashString = System.Text.Encoding.UTF8.GetString(user.PasswordHash);
                    if (BCrypt.Net.BCrypt.Verify(password, passwordHashString))
                    {
                        return user; // Login successful
                    }
                    return null;
                }
            }

            return null; // Invalid password format
        }

        // Verify password using BCrypt (new method)
        public bool VerifyPasswordHash(string password, string hashedPassword)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch
            {
                return false;
            }
        }

        // Legacy method for backward compatibility with old HMACSHA512 hashes
        private bool VerifyPasswordHashLegacy(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        // Migrate old password format to BCrypt
        private async Task MigratePasswordToBCrypt(User user, string plainPassword)
        {
            user.PasswordHash = System.Text.Encoding.UTF8.GetBytes(BCrypt.Net.BCrypt.HashPassword(plainPassword));
            user.PasswordSalt = null; // No longer needed with BCrypt
            await _context.SaveChangesAsync();
        }

        // Create password hash using BCrypt
        public string CreatePasswordHash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Legacy method signature for backward compatibility
        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            passwordHash = System.Text.Encoding.UTF8.GetBytes(hashedPassword);
            passwordSalt = new byte[0]; // Not used with BCrypt
        }

        // Create new user with BCrypt password
        public async Task<User?> CreateUser(string username, string email, string password, int roleId, string? fullName = null, string? phone = null)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _context.Users
                    .AnyAsync(u => u.Username == username || u.Email == email);

                if (existingUser)
                {
                    return null;
                }

                // Hash password with BCrypt
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

                var user = new User
                {
                    Username = username,
                    Email = email,
                    PasswordHash = System.Text.Encoding.UTF8.GetBytes(hashedPassword),
                    PasswordSalt = new byte[0], // Not used with BCrypt
                    FullName = fullName,
                    Phone = phone,
                    RoleId = roleId,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return user;
            }
            catch
            {
                return null;
            }
        }

        // Change password for existing user
        public async Task<bool> ChangePassword(int userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return false;
                }

                // Verify current password
                var loginResult = await Login(user.Username, currentPassword);
                if (loginResult == null)
                {
                    return false;
                }

                // Set new password
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
                user.PasswordHash = System.Text.Encoding.UTF8.GetBytes(hashedPassword);
                user.PasswordSalt = new byte[0]; // Not used with BCrypt

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Reset password (admin function)
        public async Task<bool> ResetPassword(int userId, string newPassword)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return false;
                }

                // Set new password
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
                user.PasswordHash = System.Text.Encoding.UTF8.GetBytes(hashedPassword);
                user.PasswordSalt = new byte[0]; // Not used with BCrypt

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
