namespace backend.DTOs.SideBar;

public class UpdateSidebarMenuDto
{
    public string Title { get; set; } = string.Empty;

    public string Route { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;

    public int? ParentId { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public List<int> RoleIds { get; set; } = new();
    public List<int> UserIds { get; set; } = new();
}
