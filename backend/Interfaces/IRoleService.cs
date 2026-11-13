using backend.DTOs.Roles;

namespace backend.Interfaces;

public interface IRoleService
{
    Task<IReadOnlyList<RoleDto>> GetAllAsync();
    Task<RoleDto> CreateAsync(SaveRoleDto dto);
    Task<RoleDto?> UpdateAsync(int id, SaveRoleDto dto);
    Task<bool> DeleteAsync(int id);
}
