using EsportsAPI.DTOs;

namespace EsportsAPI.Services;

public interface IAuthService
{
    Task<bool> Register(RegisterDto dto);

    Task<string?> Login(LoginDto dto);

    Task<bool> Promote(int userId, string providedKey);
}