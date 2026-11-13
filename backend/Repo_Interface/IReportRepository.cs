using backend.Models;

namespace backend.Repo_Interface;

public interface IReportRepository
{
    Task<List<Player>> GetPlayersAsync();
    Task<List<Attendance>> GetAttendanceAsync(DateOnly start, DateOnly end);
    Task<List<CashIn>> GetCashInsAsync(DateTime start, DateTime end);
    Task<List<CashOut>> GetCashOutsAsync(DateTime start, DateTime end);
}
