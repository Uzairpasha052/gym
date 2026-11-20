
using backend.DTOs.SideBar;
using backend.Helper;
using backend.Interface;
using backend.Models;
using backend.Repo_Interface;

namespace backend.Service;

public class SidebarService : ISidebarService
{
    private readonly ISidebarRepository _repository;
    private readonly CurrentUserService _userService;

    public SidebarService(ISidebarRepository sidebarRepository, CurrentUserService userService)
    {
        _repository = sidebarRepository;
        _userService = userService;
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

    public async Task<List<SidebarMenuDto>> GetMenuForUserAsync(int roleId, int userId)
    {
        var menus = await _repository.GetMenusForUserAsync(roleId, userId);
        var groupIds = (await _repository.GetAllMenusAsync())
            .Where(menu => menu.IsActive && menu.ParentId.HasValue)
            .Select(menu => menu.ParentId!.Value)
            .ToHashSet();
        return menus.Select(menu =>
        {
            var dto = MapMenu(menu);
            dto.IsGroup = groupIds.Contains(menu.Id);
            return dto;
        }).ToList();
    }

    public async Task<List<SidebarMenuDto>> GetAllMenusAsync()
    {
        var menus = await _repository.GetAllMenusAsync();
        var results = new List<SidebarMenuDto>(menus.Count);
        var groupIds = menus.Where(menu => menu.IsActive && menu.ParentId.HasValue)
            .Select(menu => menu.ParentId!.Value)
            .ToHashSet();
        foreach (var menu in menus)
        {
            var dto = MapMenu(menu);
            dto.IsGroup = groupIds.Contains(menu.Id);
            dto.RoleIds = await _repository.GetRoleIdsByMenuIdAsync(menu.Id);
            dto.UserIds = await _repository.GetUserIdsByMenuIdAsync(menu.Id);
            results.Add(dto);
        }
        return results;
    }

    public async Task<List<SidebarMenuDto>> GetChildMenusAsync(
        int roleId,
        int parentId)
    {
        var menus = await _repository.GetChildMenusByRoleIdAsync(
            roleId,
            parentId);

        return menus.Select(MapMenu).ToList();
    }

    public async Task<SidebarMenuDto?> GetByIdAsync(int id)
    {
        var menu = await _repository.GetByIdAsync(id);

        if (menu == null)
            return null;

        var dto = MapMenu(menu);
        dto.RoleIds = await _repository.GetRoleIdsByMenuIdAsync(menu.Id);
        dto.UserIds = await _repository.GetUserIdsByMenuIdAsync(menu.Id);
        return dto;
    }

    public async Task<(bool Success, string Message, int? Id)> CreateAsync(
       CreateSidebarMenuDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            return (false, "Title is required.", null);

        if (dto.ParentId.HasValue && string.IsNullOrWhiteSpace(dto.Route))
            return (false, "A page under a group must have an Angular route.", null);

        dto.Title = dto.Title.Trim();
        dto.Route = dto.Route?.Trim() ?? string.Empty;

        if (dto.DisplayOrder < 1)
            return (false, "Display order must be at least 1.", null);
        if (await _repository.DisplayOrderExistsAsync(dto.ParentId, dto.DisplayOrder))
            return (false, "That display order is already used by a sibling item.", null);

        if (!string.IsNullOrWhiteSpace(dto.Route) && await _repository.RouteExistsAsync(dto.Route))
            return (false, "This route already exists.", null);

        if (dto.ParentId.HasValue)
        {
            if (!await _repository.ParentExistsAsync(dto.ParentId.Value))
                return (false, "Selected parent must be an active top-level group with no route.", null);
        }

        dto.RoleIds ??= [];
        dto.UserIds ??= [];
        if (string.IsNullOrWhiteSpace(dto.Route) && (dto.RoleIds.Count > 0 || dto.UserIds.Count > 0))
            return (false, "Sidebar groups inherit visibility from their assigned child pages.", null);
        if (!await _repository.RoleIdsExistAsync(dto.RoleIds))
            return (false, "One or more selected roles are invalid or inactive.", null);
        if (!await _repository.UserIdsExistAsync(dto.UserIds))
            return (false, "One or more selected users are invalid or inactive.", null);

        var menu = new SidebarMenu
        {
            Title = dto.Title,
            Route = dto.Route,
            Icon = dto.Icon,
            ParentId = dto.ParentId,
            DisplayOrder = dto.DisplayOrder,
            IsActive = true,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _userService.UserId
            
        };

        var created = await _repository.CreateAsync(menu);

        await _repository.AssignRolesAsync(
            created.Id,
            dto.RoleIds.Distinct().ToList());
        await _repository.AssignUsersAsync(created.Id, dto.UserIds.Distinct().ToList());

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

        if (dto.ParentId.HasValue && string.IsNullOrWhiteSpace(dto.Route))
            return (false, "A page under a group must have an Angular route.");

        dto.Title = dto.Title.Trim();
        dto.Route = dto.Route?.Trim() ?? string.Empty;

        if (dto.DisplayOrder < 1)
            return (false, "Display order must be at least 1.");
        if (await _repository.DisplayOrderExistsAsync(dto.ParentId, dto.DisplayOrder, id))
            return (false, "That display order is already used by a sibling item.");

        if (!string.IsNullOrWhiteSpace(dto.Route) && await _repository.RouteExistsAsync(dto.Route, id))
            return (false, "This route already exists.");

        if (dto.ParentId == id)
            return (false, "A menu cannot be its own parent.");

        if (dto.ParentId.HasValue)
        {
            if (!await _repository.ParentExistsAsync(dto.ParentId.Value))
                return (false, "Selected parent must be an active top-level group with no route.");
        }

        dto.RoleIds ??= [];
        dto.UserIds ??= [];
        if (string.IsNullOrWhiteSpace(dto.Route) && (dto.RoleIds.Count > 0 || dto.UserIds.Count > 0))
            return (false, "Sidebar groups inherit visibility from their assigned child pages.");
        if (!await _repository.RoleIdsExistAsync(dto.RoleIds))
            return (false, "One or more selected roles are invalid or inactive.");
        if (!await _repository.UserIdsExistAsync(dto.UserIds))
            return (false, "One or more selected users are invalid or inactive.");

        menu.Title = dto.Title;
        menu.Route = dto.Route;
        menu.Icon = dto.Icon;
        menu.ParentId = dto.ParentId;
        menu.DisplayOrder = dto.DisplayOrder;
        menu.IsActive = dto.IsActive;
        menu.UpdatedAt = DateTime.UtcNow;
        menu.UpdatedBy = _userService.UserId;

        await _repository.UpdateAsync(menu);

        await _repository.AssignRolesAsync(
            menu.Id,
            dto.RoleIds.Distinct().ToList());
        await _repository.AssignUsersAsync(menu.Id, dto.UserIds.Distinct().ToList());

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
        await _repository.AssignUsersAsync(id, []);

        return (true, "Sidebar menu deleted successfully.");
    }

    private static SidebarMenuDto MapMenu(SidebarMenu menu) => new()
    {
        Id = menu.Id,
        Title = menu.Title,
        Route = menu.Route,
        Icon = menu.Icon,
        ParentId = menu.ParentId,
        DisplayOrder = menu.DisplayOrder,
        IsActive = menu.IsActive,
        IsGroup = menu.Children.Count > 0
    };


    //private static void SortMenus(List<SidebarMenuDto> menus)
    //{
    //    menus.Sort((a, b) => a.DisplayOrder.CompareTo(b.DisplayOrder));

    //    foreach (var menu in menus) 
    //    {
    //        SortMenus(menu.Children);        
    //    }

    //}
}
