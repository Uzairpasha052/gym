using backend.Data;
using backend.Models;
using backend.Repo_Interface;
using Microsoft.EntityFrameworkCore;

namespace backend.Repository;

public class ReportRepository(AppDbContext context) : IReportRepository
{
    public Task<List<Player>> GetPlayersAsync() => context.Players
        .AsNoTracking()
        .OrderBy(p => p.Name)
        .ToListAsync();

    public Task<List<Attendance>> GetAttendanceAsync(DateOnly start, DateOnly end) => context.Attendances
        .AsNoTracking()
        .Include(a => a.Player)
        .Where(a => a.AttendanceDate >= start && a.AttendanceDate <= end)
        .OrderByDescending(a => a.AttendanceDate)
        .ThenBy(a => a.Player.Name)
        .ToListAsync();

    public Task<List<CashIn>> GetCashInsAsync(DateTime start, DateTime end) => context.CashIns
        .AsNoTracking()
        .Include(c => c.Player)
        .Where(c => c.PaymentDate >= start && c.PaymentDate <= end)
        .OrderByDescending(c => c.PaymentDate)
        .ThenBy(c => c.Player.Name)
        .ToListAsync();

    public Task<List<CashOut>> GetCashOutsAsync(DateTime start, DateTime end) => context.CashOuts
        .AsNoTracking()
        .Where(c => c.PaymentDate >= start && c.PaymentDate <= end)
        .OrderByDescending(c => c.PaymentDate)
        .ToListAsync();
}
