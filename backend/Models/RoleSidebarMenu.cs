
namespace backend.Models;

public class RoleSidebarMenu
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public Role Role { get; set; } = null!;

    public int SidebarMenuId { get; set; }

    public SidebarMenu SidebarMenu { get; set; } = null!;
}

