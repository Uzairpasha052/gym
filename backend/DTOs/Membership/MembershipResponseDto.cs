
namespace backend.DTOs.Membership;

public class MembershipResponseDto
{
    public int Id { get; set; }

    public int PlayerId { get; set; }

    public string PlayerName { get; set; } = string.Empty;

    public int DurationMonths { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public decimal MonthlyFee { get; set; }

    public decimal OriginalAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal FinalAmount { get; set; }

    public string Status { get; set; } = string.Empty;

    public int? CashInId { get; set; }

    public DateTime CreatedAt { get; set; }
}
