using Role_Auth_MVC.Models.DTOs;

namespace Role_Auth_MVC.Services.Interfaces;

public interface IAuthenticationService
{
    Task<UserDto> AuthenticateAsync(LoginDto loginDto);
    Task<bool> ValidateCredentialsAsync(string username, string password);
}