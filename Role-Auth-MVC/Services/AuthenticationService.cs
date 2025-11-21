using AutoMapper;
using Role_Auth_MVC.Models.DTOs;
using Role_Auth_MVC.Models.Entities;
using Role_Auth_MVC.Repositories.Interfaces;
using Role_Auth_MVC.Services.Interfaces;

namespace Role_Auth_MVC.Services;

public class AuthenticationService(IUnitOfWork unitOfWork, IMapper mapper) : IAuthenticationService
{
    public async Task<UserDto> AuthenticateAsync(LoginDto loginDto)
    {
        var users = await unitOfWork.Users.FindAsync(u => u.Username == loginDto.Username);
        var user = users.FirstOrDefault();

        if (user == null || !user.IsActive)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            return null;

        // Load roles
        var userRoles = await unitOfWork.UserRoles.FindAsync(ur => ur.UserId == user.Id);
        var roleIds = userRoles.Select(ur => ur.RoleId).ToList();
        var roles = await unitOfWork.Roles.FindAsync(r => roleIds.Contains(r.Id));

        user.UserRoles = userRoles.Select(ur => new UserRole
        {
            UserId = ur.UserId,
            RoleId = ur.RoleId,
            Role = roles.FirstOrDefault(r => r.Id == ur.RoleId)
        }).ToList();

        return mapper.Map<UserDto>(user);
    }

    public async Task<bool> ValidateCredentialsAsync(string username, string password)
    {
        var users = await unitOfWork.Users.FindAsync(u => u.Username == username);
        var user = users.FirstOrDefault();

        return user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
    }
}