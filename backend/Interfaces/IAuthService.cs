using backend.DTOs;

namespace backend.Interfaces;

public interface IAuthService
{
    Task<bool> Register(RegisterDto request);
    Task<LoginResponseDto?> ValidateLogin(LoginDto request);
}
