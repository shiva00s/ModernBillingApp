using ModernBillingApp.Models;

namespace ModernBillingApp.Services
{
    // Service to manage user sessions and authentication state
    public class SessionService
    {
        private User? _currentUser;
        private bool _isAuthenticated = false;

        public User? CurrentUser
        {
            get => _currentUser;
            private set => _currentUser = value;
        }

        public bool IsAuthenticated => _isAuthenticated;
        public bool IsAdmin => _currentUser?.Role?.RoleName == "Admin";
        public bool IsSuperAdmin => _currentUser?.Role?.RoleName == "SuperAdmin";
        public bool IsStaff => _currentUser?.Role?.RoleName == "Staff";
        public string? UserRole => _currentUser?.Role?.RoleName;

        public event Action? OnAuthenticationChanged;

        public void SetUser(User user)
        {
            _currentUser = user;
            _isAuthenticated = true;
            OnAuthenticationChanged?.Invoke();
        }

        public void ClearUser()
        {
            _currentUser = null;
            _isAuthenticated = false;
            OnAuthenticationChanged?.Invoke();
        }
    }
}

