namespace backend.Models;

public class Attendance
{
    public int Id { get; set; }

    //Foreign Key
    public int PlayerId { get; set; }

    public DateOnly AttendanceDate { get; set; }

    public string? Status { get; set; }

    public string? Remarks { get; set; }

    public DateTime CreatedAT { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }


    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }

    // Navigation Property
    public Player Player { get; set; } = null!;

}
