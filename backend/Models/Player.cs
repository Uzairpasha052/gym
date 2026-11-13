namespace backend.Models;

public class Player
{
    
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string FName { get; set; } = string.Empty;

    public string Age { get; set; } = string.Empty;

    public string Gender { get; set; } = string.Empty;
    
    public string Contact { get; set; } = string.Empty;

    //public string stdBelt { get; set; } = string.Empty;

    public string History { get; set; } = "Fresh Member"; 

    public string Reason { get; set; } = string.Empty;

    public string Fee { get; set; } = string.Empty;

    public string Status { get; set; } = "Active";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public ICollection<CashIn> CashIns { get; set; } = new List<CashIn>();

}
