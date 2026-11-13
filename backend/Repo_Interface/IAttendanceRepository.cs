using backend.DTOs.Pagination;
using backend.Helper;
using backend.Models;

namespace backend.Repo_Interface;

public interface IAttendanceRepository
{
    Task<PagedResult<Player>> GetActivePlayersAsync(PaginationRequest request);
    Task<List<Attendance>> GetByDateAsync(DateOnly date);
    Task<Attendance?> GetByPlayerAndDateAsync(int playerId, DateOnly date);
    Task<PagedResult<Attendance>> GetHistoryAsync(PaginationRequest request, 
        DateOnly start, DateOnly end);
    Task<List<Attendance>> GetPlayerHistoryAsync(int playerId);
    Task<Attendance?> GetByIdAsync(int id);
    Task AddAsync(Attendance attendance);
    Task SaveChangesAsync();
}
