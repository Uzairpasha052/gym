
namespace backend.Models;

public class RoleSidebarMenu
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public Role Role { get; set; } = null!;

    public int SidebarMenuId { get; set; }

    public SidebarMenu SidebarMenu { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}

