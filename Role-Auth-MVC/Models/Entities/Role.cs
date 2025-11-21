namespace Role_Auth_MVC.Models.Entities;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    // Navigation - Initialize to prevent null reference
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}