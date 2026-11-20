
using backend.DTOs.SideBar;

namespace backend.Interface;

public interface ISidebarService
{
    Task<List<SidebarGroupDto>> GetParentMenusAsync(int roleId);

    Task<List<SidebarMenuDto>> GetMenuForUserAsync(int roleId, int userId);

    Task<List<SidebarMenuDto>> GetAllMenusAsync();

    Task<List<SidebarMenuDto>> GetChildMenusAsync(int roleId, int parentId);

    Task<SidebarMenuDto?> GetByIdAsync(int id);

    Task<(bool Success, string Message, int? Id)> CreateAsync(
           CreateSidebarMenuDto dto);

    Task<(bool Success, string Message)> UpdateAsync(
        int id,
        UpdateSidebarMenuDto dto);

    Task<(bool Success, string Message)> DeleteAsync(int id);
}
