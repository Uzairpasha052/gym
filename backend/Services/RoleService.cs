using System.Text.Json;
using backend.DTOs.Roles;
using backend.Helper;
using backend.Interfaces;
using backend.Model.Database;
using backend.Repo_Interface;

namespace backend.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _repository;
    private readonly IAuditLogService _audit;
    private readonly CurrentUserService _currentUser;

    public RoleService(IRoleRepository repository, IAuditLogService audit, CurrentUserService currentUser)
    {
        _repository = repository;
        _audit = audit;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<RoleDto>> GetAllAsync() => (await _repository.GetAllAsync()).Select(Map).ToList();

    public async Task<RoleDto> CreateAsync(SaveRoleDto dto)
    {
        var name = dto.Name.Trim();
        if (name.Length < 2) throw new InvalidOperationException("Role name must contain at least 2 characters.");
        if (await _repository.NameExistsAsync(name)) throw new InvalidOperationException("A role with this name already exists.");
        var role = new Role
        {
            Name = name,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUser.UserId,
            IsActive = true,
            IsDeleted = false
        };

        await _repository.AddAsync(role);
        await _repository.SaveChangesAsync();
        await _audit.LogAsync(_currentUser.UserId, "Create", "Role", role.Id.ToString(), null, JsonSerializer.Serialize(Map(role)));
        return Map(role);
    }

    public async Task<RoleDto?> UpdateAsync(int id, SaveRoleDto dto)
    {
        var role = await _repository.GetByIdAsync(id);
        if (role is null)
            return null;

        var name = dto.Name.Trim();

        if (name.Length < 2)
            throw new InvalidOperationException("Role name must contain at least 2 characters.");

        if (role.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase) && !dto.Name.Trim().Equals(role.Name, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("The Admin role cannot be renamed.");

        if (await _repository.NameExistsAsync(name, id))
            throw new InvalidOperationException("A role with this name already exists.");

        var old = JsonSerializer.Serialize(Map(role));

        role.Name = name;
        role.UpdatedAt = DateTime.UtcNow;
        role.UpdatedBy = _currentUser.UserId;

        await _repository.SaveChangesAsync();
        await _audit.LogAsync(
            _currentUser.UserId,
            "Update",
            "Role",
            id.ToString(),
            old,
            JsonSerializer.Serialize(Map(role))
        );

        return Map(role);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var role = await _repository.GetByIdAsync(id);

        if (role is null)
            return false;

        if (role.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("The Admin role cannot be deleted.");

        if (await _repository.IsAssignedAsync(id))
            throw new InvalidOperationException("This role is assigned to users and cannot be deleted.");

        var old = JsonSerializer.Serialize(Map(role));

        role.IsActive = false;
        role.IsDeleted = true;

        await _repository.DeleteAsync(role);
        await _repository.SaveChangesAsync();

        await _audit.LogAsync(
            _currentUser.UserId,
            "Delete",
            "Role",
            id.ToString(),
            old,
            null
        );

        return true;
    }

    private static RoleDto Map(Role role)
    { 
        return new()
        {
            Id = role.Id,
            Name = role.Name,
            CreatedAt = role.CreatedAt
        };
    }
}
