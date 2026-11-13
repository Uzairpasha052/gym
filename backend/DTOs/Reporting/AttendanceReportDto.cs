namespace backend.DTO.Reports;

public class AttendanceReportDto
{
    public string PlayerName { get; set; } = string.Empty;

    public string FatherName { get; set; } = string.Empty;

    //public string Belt { get; set; } = string.Empty;

    public DateOnly AttendanceDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public string Remarks { get; set; } = string.Empty;
}
