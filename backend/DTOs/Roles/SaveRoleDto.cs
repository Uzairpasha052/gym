using System.ComponentModel.DataAnnotations;

namespace backend.DTOs.Roles;

public class SaveRoleDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

}
