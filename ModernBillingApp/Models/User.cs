namespace ModernBillingApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public int UserRoleId { get; set; }
        public UserRole Role { get; set; }
    }

    public class UserRole
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
    }
}