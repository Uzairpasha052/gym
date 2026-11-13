namespace backend.DTOs.Reports;

public class PlayerReasonReportDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string FName { get; set; } = string.Empty;

    public string Age { get; set; } = string.Empty;

    public string Gender { get; set; } = string.Empty;

    public string Reason { get; set; } = string.Empty;
}
