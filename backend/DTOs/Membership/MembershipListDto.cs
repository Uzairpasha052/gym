
namespace backend.DTOs.Membership;

public class MembershipListDto
{
    public int Id { get; set; }

    public int DurationMonths { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public decimal FinalAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public string Status { get; set; } = string.Empty;
}
