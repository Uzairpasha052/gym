
using backend.Data;
using backend.Repo_Interface;
using backend.Models;
//using global::backend.Data;
//using global::backend.Models;
//using global::backend.Repo_Interface;
using Microsoft.EntityFrameworkCore;

namespace backend.Repository;

public class SidebarRepository : ISidebarRepository
{
    private readonly AppDbContext _context;

    public SidebarRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SidebarMenu>> GetParentMenusByRoleIdAsync(int roleId)
    {
        return await _context.RoleSidebarMenus
            .AsNoTracking()
            .Where(x =>
                x.RoleId == roleId &&
                x.Role.IsActive &&
                !x.Role.IsDeleted &&
                x.SidebarMenu.ParentId == null &&
                x.SidebarMenu.IsActive &&
                !x.SidebarMenu.IsDeleted)
            .Select(x => x.SidebarMenu)
            .OrderBy(x => x.DisplayOrder)
            //.ThenBy(x => x.DisplayOrder)
            .ToListAsync();
    }


    public async Task<List<SidebarMenu>> GetChildMenusByRoleIdAsync(int roleId, int parentId)
    {
        // Verify the role has access to this parent
        var hasParentAccess = await _context.RoleSidebarMenus
            .AnyAsync(x =>
                x.RoleId == roleId &&
                x.SidebarMenuId == parentId &&
                x.Role.IsActive &&
                !x.Role.IsDeleted &&
                x.SidebarMenu.IsActive &&
                !x.SidebarMenu.IsDeleted &&
                x.SidebarMenu.ParentId == null);

        if (!hasParentAccess)
            return new List<SidebarMenu>();

        return await _context.RoleSidebarMenus
            .AsNoTracking()
            .Where(x =>
                x.RoleId == roleId &&
                x.Role.IsActive &&
                !x.Role.IsDeleted &&
                x.SidebarMenu.ParentId == parentId &&
                x.SidebarMenu.IsActive &&
                !x.SidebarMenu.IsDeleted)
            .Select(x => x.SidebarMenu)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync();
    }

    public async Task<SidebarMenu?> GetByIdAsync(int id)
    {
        return await _context.SidebarMenus
            .FirstOrDefaultAsync(x =>
                x.Id == id && !x.IsDeleted);
    }

    public async Task<bool> RouteExistsAsync(string route, int? excludeId = null)
    {
        var query = _context.SidebarMenus
            .Where(x => x.Route == route && !x.IsDeleted);

        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<bool> ParentExistsAsync(int parentId)
    {
        return await _context.SidebarMenus
            .AnyAsync(x => x.Id == parentId && x.ParentId == null &&
                x.IsActive && !x.IsDeleted);
    }

    public async Task<SidebarMenu> CreateAsync(SidebarMenu menu)
    {
        _context.SidebarMenus.Add(menu);
        await _context.SaveChangesAsync();
        return menu;
    }

    public async Task UpdateAsync(SidebarMenu menu)
    {
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(SidebarMenu menu)
    {
        menu.IsDeleted = true;
        menu.IsActive = false;

        await _context.SaveChangesAsync();
    }

    public async Task<List<int>> GetRoleIdsByMenuIdAsync(int menuId)
    {
        return await _context.RoleSidebarMenus
            .Where(x => x.SidebarMenuId == menuId)
            .Select(x => x.RoleId)
            .ToListAsync();
    }

    public async Task AssignRolesAsync(int menuId, List<int> roleIds)
    {
        var existingAssingnments = await _context.RoleSidebarMenus
            .Where(x => x.SidebarMenuId == menuId)
            .ToListAsync();

        _context.RoleSidebarMenus.RemoveRange(existingAssingnments);

        var newAssignments = roleIds
            .Distinct()
            .Select(roleId => new RoleSidebarMenu
            {
                SidebarMenuId = menuId,
                RoleId = roleId
            }).ToList();

        await _context.RoleSidebarMenus.AddRangeAsync(newAssignments);
        await _context.SaveChangesAsync();

    }

    public async Task RemoveRoleAssignmentsAsync(int menuId)
    {
        var assignments = await _context.RoleSidebarMenus
            .Where(x => x.SidebarMenuId == menuId)
            .ToListAsync();

        _context.RoleSidebarMenus.RemoveRange(assignments);

        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasActiveChildrenAsync(int menuId)
    {
        return await _context.SidebarMenus
            .AnyAsync(x =>
                x.ParentId == menuId &&
                x.IsActive &&
                !x.IsDeleted);
    }

    public async Task<bool> RoleIdsExistAsync(List<int> roleIds)
    {
        var distinctIds = roleIds.Distinct().ToList();

        if (distinctIds.Count == 0)
            return true;

        var count = await _context.Roles
            .CountAsync(x =>
                distinctIds.Contains(x.Id) &&
                x.IsActive &&
                !x.IsDeleted);

        return count == distinctIds.Count;
    }

}
