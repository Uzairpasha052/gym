namespace backend.DTOs.SideBar;

public class CreateSidebarMenuDto
{
    public string Title { get; set; } = string.Empty;

    public string Route { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;

    public int? ParentId { get; set; }

    public int DisplayOrder { get; set; }

    public List<int> RoleIds { get; set; } = new();
    public List<int> UserIds { get; set; } = new();

}


//This DTO allows Admin to create a parent or child page and assign it to roles.
