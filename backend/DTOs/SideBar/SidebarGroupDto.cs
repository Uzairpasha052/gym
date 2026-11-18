namespace backend.DTOs.SideBar;

public class SidebarGroupDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Route { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }
}
