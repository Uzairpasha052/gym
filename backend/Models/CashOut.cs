namespace backend.Models;

public class CashOut
{
    
    public int Id { get; set; }

    public string Description { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }


}