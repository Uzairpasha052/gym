namespace backend.DTO.CashIn;

public class CashInListDto
{
    public int Id { get; set; }

    public int PlayerId { get; set; }

    public string PlayerName { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }
    public DateOnly? CoverageStartDate { get; set; }
    public DateOnly? CoverageEndDate { get; set; }
}
