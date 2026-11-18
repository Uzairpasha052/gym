namespace backend.DTOs.Dashboard;

public class DashboardSummaryDto
{
    public string Role { get; set; } = string.Empty;
    public string? PlayerName { get; set; }
    public decimal? MonthlyFee { get; set; }
    public int? AttendancePresent { get; set; }
    public int? AttendanceAbsent { get; set; }
    public int? AttendanceTotal { get; set; }
    public decimal? AttendanceRate { get; set; }
    public DateOnly? MembershipStartDate { get; set; }
    public DateOnly? MembershipEndDate { get; set; }
    public string? MembershipStatus { get; set; }
    public decimal? MembershipAmount { get; set; }
    public int? DaysUntilExpiry { get; set; }
    public int? ActivePlayers { get; set; }
    public int? ActiveUsers { get; set; }
    public int? PresentToday { get; set; }
    public int? AbsentToday { get; set; }
    public int? UnmarkedToday { get; set; }
    public decimal? CashInToday { get; set; }
    public decimal? CashOutToday { get; set; }
    public decimal? CashInThisMonth { get; set; }
    public decimal? CashOutThisMonth { get; set; }
    public int? ActiveMemberships { get; set; }
    public int? MembershipsExpiringSoon { get; set; }
    public List<DashboardAttendanceDto> RecentAttendance { get; set; } = [];
    public List<DashboardPaymentDto> RecentPayments { get; set; } = [];
}

public class DashboardAttendanceDto
{
    public DateOnly Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Remarks { get; set; }
}

public class DashboardPaymentDto
{
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
}
