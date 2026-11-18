namespace backend.DTOs.Membership;

public class ExpiringMemberDto
{
    public int PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public string Contact { get; set; } = string.Empty;
    public DateOnly? ExpiryDate { get; set; }
    public string CoverageType { get; set; } = string.Empty;
}
