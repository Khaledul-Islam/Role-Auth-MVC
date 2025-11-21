using Role_Auth_MVC.Models.DTOs;

namespace Role_Auth_MVC.Services.Interfaces;

public interface IUserService
{
    Task<UserDto> GetUserByIdAsync(int id);
    Task<UserDto> GetUserByUsernameAsync(string username);
    Task<UserDto> RegisterUserAsync(RegisterDto registerDto);
    Task UpdateLastLoginAsync(int userId);
}