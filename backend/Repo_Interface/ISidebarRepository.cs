using backend.Models;

namespace backend.Repo_Interface;


public interface ISidebarRepository
{
    Task<List<SidebarMenu>> GetParentMenusByRoleIdAsync(int roleId);

    Task<List<SidebarMenu>> GetChildMenusByRoleIdAsync(int roleId, int parentId);

    Task<SidebarMenu?> GetByIdAsync(int id);

    Task<bool> RouteExistsAsync(string route, int? excludeId = null);

    Task<bool> ParentExistsAsync(int parentId);

    Task<SidebarMenu> CreateAsync(SidebarMenu menu);

    Task UpdateAsync(SidebarMenu menu);

    Task SoftDeleteAsync(SidebarMenu menu);

    Task<List<int>> GetRoleIdsByMenuIdAsync(int menuId);

    Task AssignRolesAsync(int menuId, List<int> roleIds);

    Task RemoveRoleAssignmentsAsync(int menuId);

    Task<bool> HasActiveChildrenAsync(int menuId);

    Task<bool> RoleIdsExistAsync(List<int> roleIds);
}
