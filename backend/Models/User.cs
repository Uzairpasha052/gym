using backend.Model.Database;

namespace backend.Models;

public class User
{
    
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
    //public string Role { get; set; } = string.Empty; 
    // Admin, HR, Employee

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy  { get; set; }

    public DateTime UpdatedAt { get; set; } 
    public int? UpdatedBy { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

}
