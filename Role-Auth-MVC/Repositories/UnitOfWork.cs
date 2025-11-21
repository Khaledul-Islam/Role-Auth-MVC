using Role_Auth_MVC.Data;
using Role_Auth_MVC.Models.Entities;
using Role_Auth_MVC.Repositories.Interfaces;

namespace Role_Auth_MVC.Repositories;

public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    private IRepository<User> _users;
    private IRepository<Role> _roles;
    private IRepository<UserRole> _userRoles;

    public IRepository<User> Users =>
        _users ??= new Repository<User>(context);

    public IRepository<Role> Roles =>
        _roles ??= new Repository<Role>(context);

    public IRepository<UserRole> UserRoles =>
        _userRoles ??= new Repository<UserRole>(context);

    public async Task<int> CompleteAsync()
    {
        return await context.SaveChangesAsync();
    }

    public void Dispose()
    {
        context.Dispose();
    }
}