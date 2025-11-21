namespace Role_Auth_MVC.Models.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string FullName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    // Navigation - Initialize to prevent null reference
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}