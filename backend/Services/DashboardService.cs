using backend.Data;
using backend.DTOs.Dashboard;
using backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class DashboardService(AppDbContext db) : IDashboardService
{
    public async Task<DashboardSummaryDto> GetAdminAsync()
    {
        var summary = await GetOperationsSummaryAsync("Admin");
        var today = DateOnly.FromDateTime(DateTime.Today);
        summary.ActiveUsers = await db.Users.CountAsync(u => u.IsActive && !u.IsDeleted);
        summary.ActiveMemberships = await db.PlayerMemberships.CountAsync(m => m.StartDate <= today && m.EndDate >= today && m.Status != "Cancelled");
        summary.MembershipsExpiringSoon = await db.PlayerMemberships.CountAsync(m => m.EndDate >= today && m.EndDate <= today.AddDays(7) && m.Status != "Cancelled");
        var monthStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var nextMonth = monthStart.AddMonths(1);
        summary.CashInThisMonth = await db.CashIns.Where(c => c.PaymentDate >= monthStart && c.PaymentDate < nextMonth).SumAsync(c => (decimal?)c.Amount) ?? 0;
        summary.CashOutThisMonth = await db.CashOuts.Where(c => c.PaymentDate >= monthStart && c.PaymentDate < nextMonth).SumAsync(c => (decimal?)c.Amount) ?? 0;
        return summary;
    }

    public async Task<DashboardSummaryDto> GetReceptionistAsync() => await GetOperationsSummaryAsync("Receptionist");

    private async Task<DashboardSummaryDto> GetOperationsSummaryAsync(string role)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var dateStart = DateTime.Today;
        var dateEnd = dateStart.AddDays(1);
        var playersCount = await db.Players.CountAsync(p => p.IsActive && !p.IsDeleted && p.Status == "Active");
        var attendances = await db.Attendances.AsNoTracking().Where(a => a.AttendanceDate == today).ToListAsync();
        var cashIn = await db.CashIns.Where(c => c.PaymentDate >= dateStart && c.PaymentDate < dateEnd).SumAsync(c => (decimal?)c.Amount) ?? 0;
        var cashOut = await db.CashOuts.Where(c => c.PaymentDate >= dateStart && c.PaymentDate < dateEnd).SumAsync(c => (decimal?)c.Amount) ?? 0;
        return new DashboardSummaryDto
        {
            Role = role,
            ActivePlayers = playersCount,
            PresentToday = attendances.Count(a => a.Status == "Present"),
            AbsentToday = attendances.Count(a => a.Status == "Absent"),
            UnmarkedToday = Math.Max(0, playersCount - attendances.Count),
            CashInToday = cashIn,
            CashOutToday = cashOut
        };
    }

    public async Task<DashboardSummaryDto?> GetMemberAsync(int userId)
    {
        var user = await db.Users.AsNoTracking().Include(u => u.Player)
            .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive && !u.IsDeleted);
        if (user?.Player is null) return null;
        var player = user.Player;
        var today = DateOnly.FromDateTime(DateTime.Today);
        var playerAttendance = db.Attendances.AsNoTracking().Where(a => a.PlayerId == player.Id);
        var present = await playerAttendance.CountAsync(a => a.Status == "Present");
        var absent = await playerAttendance.CountAsync(a => a.Status == "Absent");
        var membership = await db.PlayerMemberships.AsNoTracking().Where(m => m.PlayerId == player.Id && m.Status != "Cancelled")
            .OrderByDescending(m => m.EndDate).FirstOrDefaultAsync();
        var payments = await db.CashIns.AsNoTracking().Where(c => c.PlayerId == player.Id).OrderByDescending(c => c.PaymentDate).Take(8)
            .Select(c => new DashboardPaymentDto { Date = c.PaymentDate, Amount = c.Amount }).ToListAsync();
        decimal? monthlyFee = decimal.TryParse(player.Fee, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var fee) ? fee : null;
        var currentMembership = membership is not null && membership.StartDate <= today && membership.EndDate >= today;
        return new DashboardSummaryDto
        {
            Role = "Member", PlayerName = player.Name, MonthlyFee = monthlyFee,
            AttendancePresent = present, AttendanceAbsent = absent, AttendanceTotal = present + absent,
            AttendanceRate = present + absent == 0 ? 0 : Math.Round(present * 100m / (present + absent), 1),
            MembershipStartDate = membership?.StartDate, MembershipEndDate = membership?.EndDate,
            MembershipStatus = membership is null ? "No membership" : currentMembership ? "Active" : membership.StartDate > today ? "Scheduled" : "Expired",
            MembershipAmount = membership?.FinalAmount, DaysUntilExpiry = membership is not null && membership.EndDate >= today ? membership.EndDate.DayNumber - today.DayNumber : null,
            RecentAttendance = await playerAttendance.OrderByDescending(a => a.AttendanceDate).Take(10).Select(a => new DashboardAttendanceDto { Date = a.AttendanceDate, Status = a.Status ?? "Unknown", Remarks = a.Remarks }).ToListAsync(),
            RecentPayments = payments
        };
    }
}
