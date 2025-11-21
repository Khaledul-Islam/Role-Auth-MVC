using AutoMapper;
using Role_Auth_MVC.Models.DTOs;
using Role_Auth_MVC.Models.Entities;
using Role_Auth_MVC.Repositories.Interfaces;
using Role_Auth_MVC.Services.Interfaces;

namespace Role_Auth_MVC.Services;

public class UserService(IUnitOfWork unitOfWork, IMapper mapper) : IUserService
{
    public async Task<UserDto> GetUserByIdAsync(int id)
    {
        var user = await unitOfWork.Users.GetByIdAsync(id);
        return mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> GetUserByUsernameAsync(string username)
    {
        var user = await unitOfWork.Users.SingleOrDefaultAsync(u => u.Username == username);

        if (user != null)
        {
            var userRoles = await unitOfWork.UserRoles.FindAsync(ur => ur.UserId == user.Id);
            var roleIds = userRoles.Select(ur => ur.RoleId).ToList();
            var roles = await unitOfWork.Roles.FindAsync(r => roleIds.Contains(r.Id));

            user.UserRoles = userRoles.Select(ur => new UserRole
            {
                UserId = ur.UserId,
                RoleId = ur.RoleId,
                Role = roles.FirstOrDefault(r => r.Id == ur.RoleId)
            }).ToList();
        }

        return mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> RegisterUserAsync(RegisterDto registerDto)
    {
        var user = mapper.Map<User>(registerDto);
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

        await unitOfWork.Users.AddAsync(user);
        await unitOfWork.CompleteAsync();

        // Assign Customer role by default
        var customerRole = (await unitOfWork.Roles.FindAsync(r => r.Name == "Customer")).FirstOrDefault();
        if (customerRole != null)
        {
            await unitOfWork.UserRoles.AddAsync(new UserRole
            {
                UserId = user.Id,
                RoleId = customerRole.Id
            });
            await unitOfWork.CompleteAsync();
        }

        return await GetUserByIdAsync(user.Id);
    }

    public async Task UpdateLastLoginAsync(int userId)
    {
        var user = await unitOfWork.Users.GetByIdAsync(userId);
        if (user != null)
        {
            user.LastLoginAt = DateTime.UtcNow;
            unitOfWork.Users.Update(user);
            await unitOfWork.CompleteAsync();
        }
    }
}