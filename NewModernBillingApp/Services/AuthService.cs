using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using NewModernBillingApp.Data;
using NewModernBillingApp.Models;

namespace NewModernBillingApp.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AuthService> _logger;

        public AuthService(AppDbContext context, ILogger<AuthService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

                if (user == null)
                {
                    _logger.LogWarning("Login attempt failed: User '{Username}' not found or inactive", username);
                    return null;
                }

                if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                {
                    _logger.LogWarning("Login attempt failed: Invalid password for user '{Username}'", username);
                    return null;
                }

                // Update last login date
                user.LastLoginDate = DateTime.Now;
                await _context.SaveChangesAsync();

                _logger.LogInformation("User '{Username}' logged in successfully", username);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during authentication for user '{Username}'", username);
                return null;
            }
        }

        public async Task<bool> CreateUserAsync(string username, string email, string password, int roleId, string? fullName = null, string? phone = null)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _context.Users
                    .AnyAsync(u => u.Username == username || u.Email == email);

                if (existingUser)
                {
                    _logger.LogWarning("User creation failed: Username '{Username}' or email '{Email}' already exists", username, email);
                    return false;
                }

                // Check if role exists
                var role = await _context.UserRoles.FindAsync(roleId);
                if (role == null)
                {
                    _logger.LogWarning("User creation failed: Role with ID '{RoleId}' not found", roleId);
                    return false;
                }

                var user = new User
                {
                    Username = username,
                    Email = email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                    FullName = fullName,
                    Phone = phone,
                    RoleId = roleId,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User '{Username}' created successfully with role '{RoleName}'", username, role.RoleName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user '{Username}'", username);
                return false;
            }
        }

        public async Task<bool> UpdateUserAsync(int userId, string? username = null, string? email = null, string? fullName = null, string? phone = null, int? roleId = null, bool? isActive = null)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User update failed: User with ID '{UserId}' not found", userId);
                    return false;
                }

                if (!string.IsNullOrEmpty(username) && username != user.Username)
                {
                    var usernameExists = await _context.Users.AnyAsync(u => u.Username == username && u.Id != userId);
                    if (usernameExists)
                    {
                        _logger.LogWarning("User update failed: Username '{Username}' already exists", username);
                        return false;
                    }
                    user.Username = username;
                }

                if (!string.IsNullOrEmpty(email) && email != user.Email)
                {
                    var emailExists = await _context.Users.AnyAsync(u => u.Email == email && u.Id != userId);
                    if (emailExists)
                    {
                        _logger.LogWarning("User update failed: Email '{Email}' already exists", email);
                        return false;
                    }
                    user.Email = email;
                }

                if (!string.IsNullOrEmpty(fullName))
                    user.FullName = fullName;

                if (!string.IsNullOrEmpty(phone))
                    user.Phone = phone;

                if (roleId.HasValue)
                {
                    var role = await _context.UserRoles.FindAsync(roleId.Value);
                    if (role == null)
                    {
                        _logger.LogWarning("User update failed: Role with ID '{RoleId}' not found", roleId.Value);
                        return false;
                    }
                    user.RoleId = roleId.Value;
                }

                if (isActive.HasValue)
                    user.IsActive = isActive.Value;

                await _context.SaveChangesAsync();

                _logger.LogInformation("User '{Username}' updated successfully", user.Username);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID '{UserId}'", userId);
                return false;
            }
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("Password change failed: User with ID '{UserId}' not found", userId);
                    return false;
                }

                if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
                {
                    _logger.LogWarning("Password change failed: Invalid current password for user '{Username}'", user.Username);
                    return false;
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Password changed successfully for user '{Username}'", user.Username);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user with ID '{UserId}'", userId);
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(int userId, string newPassword)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("Password reset failed: User with ID '{UserId}' not found", userId);
                    return false;
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Password reset successfully for user '{Username}'", user.Username);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for user with ID '{UserId}'", userId);
                return false;
            }
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                return await _context.Users
                    .Include(u => u.Role)
                    .OrderBy(u => u.Username)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return new List<User>();
            }
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            try
            {
                return await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Id == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user with ID '{UserId}'", userId);
                return null;
            }
        }

        public async Task<List<UserRole>> GetAllRolesAsync()
        {
            try
            {
                return await _context.UserRoles
                    .OrderBy(r => r.RoleName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all roles");
                return new List<UserRole>();
            }
        }
    }

    public class PermissionService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PermissionService> _logger;

        public PermissionService(AppDbContext context, ILogger<PermissionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> HasPermissionAsync(int userId, string permissionKey, string action = "Read")
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Role)
                    .ThenInclude(r => r.Permissions)
                    .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);

                if (user == null || user.Role == null)
                    return false;

                var permission = user.Role.Permissions
                    .FirstOrDefault(p => p.PermissionKey == permissionKey);

                if (permission == null)
                    return false;

                return action.ToLower() switch
                {
                    "create" => permission.CanCreate,
                    "read" => permission.CanRead,
                    "update" => permission.CanUpdate,
                    "delete" => permission.CanDelete,
                    _ => permission.CanRead
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking permission '{PermissionKey}' for user '{UserId}'", permissionKey, userId);
                return false;
            }
        }

        public async Task<List<string>> GetUserPermissionsAsync(int userId)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Role)
                    .ThenInclude(r => r.Permissions)
                    .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);

                if (user == null || user.Role == null)
                    return new List<string>();

                return user.Role.Permissions
                    .Where(p => p.CanRead)
                    .Select(p => p.PermissionKey)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting permissions for user '{UserId}'", userId);
                return new List<string>();
            }
        }

        public async Task<bool> UpdateRolePermissionAsync(int roleId, string permissionKey, bool canCreate, bool canRead, bool canUpdate, bool canDelete)
        {
            try
            {
                var permission = await _context.RolePermissions
                    .FirstOrDefaultAsync(p => p.RoleId == roleId && p.PermissionKey == permissionKey);

                if (permission == null)
                {
                    // Create new permission
                    permission = new RolePermission
                    {
                        RoleId = roleId,
                        PermissionKey = permissionKey,
                        CanCreate = canCreate,
                        CanRead = canRead,
                        CanUpdate = canUpdate,
                        CanDelete = canDelete
                    };
                    _context.RolePermissions.Add(permission);
                }
                else
                {
                    // Update existing permission
                    permission.CanCreate = canCreate;
                    permission.CanRead = canRead;
                    permission.CanUpdate = canUpdate;
                    permission.CanDelete = canDelete;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating permission '{PermissionKey}' for role '{RoleId}'", permissionKey, roleId);
                return false;
            }
        }

        public async Task<List<RolePermission>> GetRolePermissionsAsync(int roleId)
        {
            try
            {
                return await _context.RolePermissions
                    .Where(p => p.RoleId == roleId)
                    .OrderBy(p => p.PermissionKey)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting permissions for role '{RoleId}'", roleId);
                return new List<RolePermission>();
            }
        }

        public async Task<bool> CreateRoleAsync(string roleName, string? description = null)
        {
            try
            {
                var existingRole = await _context.UserRoles
                    .AnyAsync(r => r.RoleName == roleName);

                if (existingRole)
                {
                    _logger.LogWarning("Role creation failed: Role '{RoleName}' already exists", roleName);
                    return false;
                }

                var role = new UserRole
                {
                    RoleName = roleName,
                    Description = description,
                    IsSystemRole = false
                };

                _context.UserRoles.Add(role);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Role '{RoleName}' created successfully", roleName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role '{RoleName}'", roleName);
                return false;
            }
        }

        public async Task<bool> UpdateRoleAsync(int roleId, string roleName, string? description = null)
        {
            try
            {
                var role = await _context.UserRoles.FindAsync(roleId);
                if (role == null)
                {
                    _logger.LogWarning("Role update failed: Role with ID '{RoleId}' not found", roleId);
                    return false;
                }

                if (role.IsSystemRole)
                {
                    _logger.LogWarning("Role update failed: Cannot modify system role '{RoleName}'", role.RoleName);
                    return false;
                }

                var existingRole = await _context.UserRoles
                    .AnyAsync(r => r.RoleName == roleName && r.Id != roleId);

                if (existingRole)
                {
                    _logger.LogWarning("Role update failed: Role '{RoleName}' already exists", roleName);
                    return false;
                }

                role.RoleName = roleName;
                role.Description = description;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Role '{RoleName}' updated successfully", roleName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role with ID '{RoleId}'", roleId);
                return false;
            }
        }

        public async Task<bool> DeleteRoleAsync(int roleId)
        {
            try
            {
                var role = await _context.UserRoles
                    .Include(r => r.Users)
                    .FirstOrDefaultAsync(r => r.Id == roleId);

                if (role == null)
                {
                    _logger.LogWarning("Role deletion failed: Role with ID '{RoleId}' not found", roleId);
                    return false;
                }

                if (role.IsSystemRole)
                {
                    _logger.LogWarning("Role deletion failed: Cannot delete system role '{RoleName}'", role.RoleName);
                    return false;
                }

                if (role.Users.Any())
                {
                    _logger.LogWarning("Role deletion failed: Role '{RoleName}' has users assigned", role.RoleName);
                    return false;
                }

                _context.UserRoles.Remove(role);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Role '{RoleName}' deleted successfully", role.RoleName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role with ID '{RoleId}'", roleId);
                return false;
            }
        }
    }

    public class SessionService
    {
        private User? _currentUser;
        private List<string>? _userPermissions;

        public User? CurrentUser => _currentUser;

        public bool IsLoggedIn => _currentUser != null;

        public string? CurrentUserName => _currentUser?.Username;

        public string? CurrentUserRole => _currentUser?.Role?.RoleName;

        public int? CurrentUserId => _currentUser?.Id;

        public void SetCurrentUser(User user)
        {
            _currentUser = user;
            _userPermissions = null; // Reset permissions cache
        }

        public void ClearCurrentUser()
        {
            _currentUser = null;
            _userPermissions = null;
        }

        public void SetUserPermissions(List<string> permissions)
        {
            _userPermissions = permissions;
        }

        public bool HasPermission(string permissionKey)
        {
            return _userPermissions?.Contains(permissionKey) ?? false;
        }

        public List<string> GetPermissions()
        {
            return _userPermissions ?? new List<string>();
        }

        public bool IsSuperAdmin()
        {
            return _currentUser?.Role?.RoleName == "SuperAdmin";
        }

        public bool IsAdmin()
        {
            return _currentUser?.Role?.RoleName == "Admin" || IsSuperAdmin();
        }

        public bool IsStaff()
        {
            return _currentUser?.Role?.RoleName == "Staff" || IsAdmin();
        }
    }
}
