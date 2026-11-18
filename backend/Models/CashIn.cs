namespace backend.Models;

public class CashIn
{
    
    public int Id { get; set; }

    public int PlayerId { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    // Inclusive dates covered by this payment. A standard fee covers 30 days.
    public DateOnly? CoverageStartDate { get; set; }
    public DateOnly? CoverageEndDate { get; set; }
    public int? CreatedBy { get; set; }

    public int? MembershipId { get; set; }


    // Navigation Property
    public PlayerMembership? Membership { get; set; }
    public Player Player { get; set; } = null!;
}
