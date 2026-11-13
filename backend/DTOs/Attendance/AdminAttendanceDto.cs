namespace backend.DTO.Attendance;

public class AdminAttendanceDto
{
    public int AttendanceId { get; set; }

    public int PlayerId { get; set; }
    
    public string PlayerName { get; set; } = string.Empty;

    //public string PlayerBelt { get; set; } = string.Empty;

    // Timestamp when this attendance record was first saved.
    public DateTime AttendanceDate { get; set; }

    public string? Remarks { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    // public DateTime? CheckInTime { get; set; }
    // public DateTime? CheckOutTime { get; set; }
    // public string Email { get; set; } = string.Empty;

}
