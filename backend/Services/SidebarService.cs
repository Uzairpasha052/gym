
using backend.DTOs.SideBar;
using backend.Interface;
using backend.Models;
using backend.Repo_Interface;

namespace backend.Service;

public class SidebarService : ISidebarService
{
    private readonly ISidebarRepository _repository;

    public SidebarService(ISidebarRepository sidebarRepository)
    {
        _repository = sidebarRepository;
    }

    public async Task<List<SidebarGroupDto>> GetParentMenusAsync(int roleId)
    {
        var menus = await _repository.GetParentMenusByRoleIdAsync(roleId);

        return menus.Select(x => new SidebarGroupDto
        {
            Id = x.Id,
            Title = x.Title,
            Route = x.Route,
            Icon = x.Icon,
            DisplayOrder = x.DisplayOrder
        }).ToList();
        //var menuDtos = menus
        //    .Select(menu => new SidebarMenuDto
        //    {
        //        Id = menu.Id,
        //        Title = menu.Title,
        //        Route = menu.Route,
        //        Icon = menu.Icon,
        //        ParentId = menu.ParentId,
        //        DisplayOrder = menu.DisplayOrder,
        //        Children = new List<SidebarMenuDto>()
        //    })
        //    .ToList();

        // Create lookup for quick access
        //    var menuLookup = menuDtos.ToDictionary(x => x.Id);
        //    var rootMenus = new List<SidebarMenuDto>();

        //    foreach(var menu in menuDtos)
        //    {
        //        if(menu.ParentId.HasValue && menuLookup.TryGetValue(menu.ParentId.Value, out var parent))
        //        {
        //            parent.Children.Add(menu);
        //        }
        //        else
        //        {
        //            rootMenus.Add(menu);
        //        }
        //    }
        //    SortMenus(rootMenus);
        //    return rootMenus;
    }

    public async Task<List<SidebarMenuDto>> GetChildMenusAsync(
        int roleId,
        int parentId)
    {
        var menus = await _repository.GetChildMenusByRoleIdAsync(
            roleId,
            parentId);

        return menus.Select(x => new SidebarMenuDto
        {
            Id = x.Id,
            Title = x.Title,
            Route = x.Route,
            Icon = x.Icon,
            ParentId = x.ParentId,
            DisplayOrder = x.DisplayOrder
        }).ToList();
    }

    public async Task<SidebarMenuDto?> GetByIdAsync(int id)
    {
        var menu = await _repository.GetByIdAsync(id);

        if (menu == null)
            return null;

        return new SidebarMenuDto
        {
            Id = menu.Id,
            Title = menu.Title,
            Route = menu.Route,
            Icon = menu.Icon,
            ParentId = menu.ParentId,
            DisplayOrder = menu.DisplayOrder,
            IsActive = menu.IsActive,
            RoleIds = await _repository.GetRoleIdsByMenuIdAsync(menu.Id)
        };
    }

    public async Task<(bool Success, string Message, int? Id)> CreateAsync(
       CreateSidebarMenuDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            return (false, "Title is required.", null);

        if (string.IsNullOrWhiteSpace(dto.Route))
            return (false, "Route is required.", null);

        dto.Title = dto.Title.Trim();
        dto.Route = dto.Route.Trim();

        if (await _repository.RouteExistsAsync(dto.Route))
            return (false, "This route already exists.", null);

        if (dto.ParentId.HasValue)
        {
            if (!await _repository.ParentExistsAsync(dto.ParentId.Value))
                return (false, "Selected parent does not exist or is inactive.", null);
        }

        if (!await _repository.RoleIdsExistAsync(dto.RoleIds))
            return (false, "One or more selected roles are invalid or inactive.", null);

        var menu = new SidebarMenu
        {
            Title = dto.Title,
            Route = dto.Route,
            Icon = dto.Icon,
            ParentId = dto.ParentId,
            DisplayOrder = dto.DisplayOrder,
            IsActive = true,
            IsDeleted = false
        };

        var created = await _repository.CreateAsync(menu);

        await _repository.AssignRolesAsync(
            created.Id,
            dto.RoleIds.Distinct().ToList());

        return (true, "Sidebar menu created successfully.", created.Id);
    }

    public async Task<(bool Success, string Message)> UpdateAsync(
        int id,
        UpdateSidebarMenuDto dto)
    {
        var menu = await _repository.GetByIdAsync(id);

        if (menu == null)
            return (false, "Sidebar menu not found.");

        if (string.IsNullOrWhiteSpace(dto.Title))
            return (false, "Title is required.");

        if (string.IsNullOrWhiteSpace(dto.Route))
            return (false, "Route is required.");

        dto.Title = dto.Title.Trim();
        dto.Route = dto.Route.Trim();

        if (await _repository.RouteExistsAsync(dto.Route, id))
            return (false, "This route already exists.");

        if (dto.ParentId == id)
            return (false, "A menu cannot be its own parent.");

        if (dto.ParentId.HasValue)
        {
            if (!await _repository.ParentExistsAsync(dto.ParentId.Value))
                return (false, "Selected parent does not exist or is inactive.");
        }

        if (!await _repository.RoleIdsExistAsync(dto.RoleIds))
            return (false, "One or more selected roles are invalid or inactive.");

        menu.Title = dto.Title;
        menu.Route = dto.Route;
        menu.Icon = dto.Icon;
        menu.ParentId = dto.ParentId;
        menu.DisplayOrder = dto.DisplayOrder;
        menu.IsActive = dto.IsActive;

        await _repository.UpdateAsync(menu);

        await _repository.AssignRolesAsync(
            menu.Id,
            dto.RoleIds.Distinct().ToList());

        return (true, "Sidebar menu updated successfully.");
    }

    public async Task<(bool Success, string Message)> DeleteAsync(int id)
    {
        var menu = await _repository.GetByIdAsync(id);

        if (menu == null)
            return (false, "Sidebar menu not found.");

        if (await _repository.HasActiveChildrenAsync(id))
            return (false, "Delete or move the child menus before deleting this parent.");

        await _repository.SoftDeleteAsync(menu);

        await _repository.RemoveRoleAssignmentsAsync(id);

        return (true, "Sidebar menu deleted successfully.");
    }


    //private static void SortMenus(List<SidebarMenuDto> menus)
    //{
    //    menus.Sort((a, b) => a.DisplayOrder.CompareTo(b.DisplayOrder));

    //    foreach (var menu in menus) 
    //    {
    //        SortMenus(menu.Children);        
    //    }

    //}
}
