
namespace backend.Models;

public class PlayerMembership
{
    public int Id { get; set; }

    public int PlayerId { get; set; }

    public int DurationMonths { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public decimal MonthlyFee { get; set; }

    public decimal OriginalAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal FinalAmount { get; set; }

    // Active, Scheduled, Expired, Cancelled
    public string Status { get; set; } = "Active";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int? CreatedBy { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public int? UpdatedBy { get; set; }

    // Navigation properties
    public Player Player { get; set; } = null!;

    public CashIn? CashIn { get; set; }

}
