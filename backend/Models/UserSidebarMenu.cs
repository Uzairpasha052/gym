namespace backend.Models;

public class UserSidebarMenu
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int SidebarMenuId { get; set; }
    public SidebarMenu SidebarMenu { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}
