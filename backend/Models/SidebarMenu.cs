namespace backend.Models;

public class SidebarMenu
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Route { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;

    // Admin or Cash
    //public string GroupName { get; set; } = string.Empty;

    // NULL means this is a parent.
    // A value means this is a child of another menu.
    public int? ParentId { get; set; }

    public SidebarMenu? Parent { get; set; }

    public ICollection<SidebarMenu> Children { get; set; }
        = new List<SidebarMenu>();

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; } = false;

    public ICollection<RoleSidebarMenu> RoleSidebarMenus { get; set; }
        = new List<RoleSidebarMenu>();
}

