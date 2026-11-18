using backend.DTOs;
using backend.DTOs.Pagination;
using backend.Helper;
using backend.Interfaces;
using backend.Models;
using backend.Repo_Interface;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;

namespace backend.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IJwtService _jwtService;
    private readonly CurrentUserService _currentUser;
    private readonly PasswordHasher<User> _passwordHasher = new();
    private readonly IAuditLogService _logService;

    public UserService(IUserRepository repository, IJwtService jwtService, 
        CurrentUserService currentUser, IAuditLogService logService)
    {
        _repository = repository;
        _jwtService = jwtService;
        _currentUser = currentUser;
        _logService = logService;
    }

    public async Task<PagedResult<UserDto>> GetAllUsersAsync(PaginationRequest request)
    {
        var users = await _repository.GetAllAsync(request);

        //var activeUsers = users.Items.Where(u => u.IsActive).ToList();
        //return users.Select(MapToDto).ToList();
        return new PagedResult<UserDto>
        {
            //Items = users.Items.Select(u => new UserDto
            //{
            //    Id = u.Id,
            //    FullName = u.FullName,
            //    Email = u.Email,
            //    RoleId = u.RoleId,
            //    RoleName = u.Role?.Name ?? string.Empty,
            //    CreatedAt = u.CreatedAt
            //}).ToList(),
            Items = users.Items.Select(MapToDto).ToList(),

            TotalCount = users.TotalCount,
            PageNumber = users.PageNumber,
            PageSize = users.PageSize,
        };
    }

    public async Task<bool> DeleteUser(int id)
    {
        if(id == _currentUser.UserId)
        {
            throw new InvalidOperationException(
                "You cannot delete your own account.");
        }

        var user = await _repository.GetByIdAsync(id);
        if (user == null) { return false; }

        var oldValues = JsonSerializer.Serialize(new
        {
            user.FullName,
            user.Email,
            user.RoleId,
            user.IsActive
        });

        user.IsActive = false;
        user.IsDeleted = true;
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = _currentUser.UserId;

        var result = await _repository.UpdateAsync(user);
        if (!result) return false;

        var newValues = JsonSerializer.Serialize(new
        {
            user.FullName,
            user.Email,
            user.RoleId,
            user.IsActive
        });

        await _logService.LogAsync(
            _currentUser.UserId,
            "Delete",
            "User",
            user.Id.ToString(),
            oldValues,
            newValues
        );
        
        return result;
        //return user is not null && await _repository.DeleteAsync(user);
    }

    public async Task<UserDto?> UpdateUser(int id, UpdateUserDto dto)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user is null) return null;

        var role = await _repository.GetRoleByIdAsync(dto.RoleId);
        if (role is null) return null;

        Player? player = null;
        if (role.Name.Equals("Member", StringComparison.OrdinalIgnoreCase))
        {
            if (dto.PlayerId is null) throw new ArgumentException("Select the player profile for this member account.");
            player = await _repository.GetPlayerByIdAsync(dto.PlayerId.Value);
            if (player is null) throw new ArgumentException("The selected player profile is unavailable.");
            if (await _repository.IsPlayerLinkedAsync(player.Id, user.Id))
                throw new ArgumentException("This player already has a login account.");
        }
        else if (dto.PlayerId.HasValue)
            throw new ArgumentException("Only Member accounts can be linked to a player profile.");

        var oldValues = JsonSerializer.Serialize(new
        {
            user.FullName,
            user.Email,
            user.RoleId,
            Role = user.Role.Name,
            user.PlayerId,
            Player = user.Player?.Name
        });

        user.FullName = dto.FullName;
        user.Email = dto.Email;
        user.RoleId = dto.RoleId;
        user.Role = role;
        user.PlayerId = player?.Id;
        user.Player = player;

        if (!await _repository.UpdateAsync(user)) return null;
        var result = MapToDto(user);

        var newValues = JsonSerializer.Serialize(new
        {
            result.FullName,
            result.Email,
            result.RoleId,
            result.RoleName,
            result.PlayerId,
            result.PlayerName
        });

        await _logService.LogAsync(
            _currentUser.UserId,
            "Update",
            "User",
            user.Id.ToString(),
            oldValues,
            newValues
        );

        return result;
        //return await _repository.UpdateAsync(user) ? MapToDto(user) : null;
    }

    private static UserDto MapToDto(User user) => new()
    {
        Id = user.Id,
        FullName = user.FullName,
        Email = user.Email,
        RoleId = user.RoleId,
        RoleName = user.Role?.Name ?? string.Empty,
        PlayerId = user.PlayerId,
        PlayerName = user.Player?.Name,
        CreatedAt = user.CreatedAt
    };
}
