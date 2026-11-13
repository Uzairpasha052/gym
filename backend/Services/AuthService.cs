using backend.DTOs;
using backend.Helper;
using backend.Interfaces;
using backend.Models;
using backend.Repo_Interface;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;

namespace backend.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _repository;
    private readonly IJwtService _jwtService;
    private readonly CurrentUserService _currentUser;
    private readonly PasswordHasher<User> _passwordHasher = new();
    private readonly IAuditLogService _logService;


    public AuthService(IUserRepository repository, IJwtService jwtService, 
        CurrentUserService currentUser, IAuditLogService logService)
    {
        _repository = repository;
        _jwtService = jwtService;
        _currentUser = currentUser;
        _logService = logService;
    }

    public async Task<bool> Register(RegisterDto request)
    {
        var role = await _repository.GetRoleByIdAsync(request.RoleId);
        var existingUser = await _repository.GetByEmailAsync(request.Email);
        if (role is null || existingUser is not null) return false;

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            RoleId = role.Id,
            Role = role,
            CreatedBy = _currentUser.UserId
        };
        user.Password = _passwordHasher.HashPassword(user, request.Password);

        var result = await _repository.CreateAsync(user);

        var newValues = JsonSerializer.Serialize(new
        {
            result.Id,
            result.FullName,
            Mail = result.Email,
            Role = result.Role.Name,
        });

        await _logService.LogAsync(
            _currentUser.UserId,
            "Register",
            "User",
            result.Id.ToString(),
            null,
            newValues
        );

        return true;
    }


    public async Task<LoginResponseDto?> ValidateLogin(LoginDto request)
    {
        var user = await _repository.GetByEmailAsync(request.Email);
        if (user is null || _passwordHasher.VerifyHashedPassword(user, user.Password, request.Password) == PasswordVerificationResult.Failed)
            return null;

        var newValues = JsonSerializer.Serialize(new
        {
            user.Id,
            user.FullName,
            Mail = user.Email,
            Role = user.Role.Name,
        });

        await _logService.LogAsync(
            user.Id,
            "Login",
            "User",
            user.Id.ToString(),
            null,
            newValues
        );

        return new LoginResponseDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            RoleId = user.RoleId,
            Token = _jwtService.GenerateToken(user, request.RememberMe)
        };
    }

}
