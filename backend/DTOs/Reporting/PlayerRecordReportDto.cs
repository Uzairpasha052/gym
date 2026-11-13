namespace backend.DTOs.Reports;

public class PlayerRecordReportDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string FName { get; set; } = string.Empty;

    public string Age { get; set; } = string.Empty;

    public string Gender { get; set; } = string.Empty;

    public string Contact { get; set; } = string.Empty;

    public string Belt { get; set; } = string.Empty;

    public string History { get; set; } = string.Empty;

    public string Fee { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
