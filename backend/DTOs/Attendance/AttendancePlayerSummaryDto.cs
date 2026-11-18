namespace backend.DTO.Attendance;

public class AttendancePlayerSummaryDto
{
    public string PlayerName { get; set; } = string.Empty;
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public int TotalCount => PresentCount + AbsentCount;
}
