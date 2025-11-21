using Role_Auth_MVC.Models.Entities;

namespace Role_Auth_MVC.Repositories.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<User> Users { get; }
    IRepository<Role> Roles { get; }
    IRepository<UserRole> UserRoles { get; }
    Task<int> CompleteAsync();
}