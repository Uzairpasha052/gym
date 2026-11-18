using backend.Data;
using backend.DTOs.Pagination;
using backend.DTO.Attendance;
using backend.Helper;
using backend.Models;
using backend.Repo_Interface;
using Microsoft.EntityFrameworkCore;

namespace backend.Repository;

public class AttendanceRepository : IAttendanceRepository
{
    private readonly AppDbContext context;
    public AttendanceRepository(AppDbContext _context)
    {
        context = _context;
    }
    public async Task<PagedResult<Player>> GetActivePlayersAsync(PaginationRequest request)
    {
        int pageNumber = Math.Max(1, request.PageNumber);
        int pageSize = Math.Clamp(request.PageSize, 1, 100);

        var query = context.Players
        .AsNoTracking()
        .Where(p => p.Status == "Active")
        .OrderBy(p => p.Name);
        //.ToListAsync();

        int totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Player>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public Task<List<Attendance>> GetByDateAsync(DateOnly date) => context.Attendances
        .AsNoTracking()
        .Where(a => a.AttendanceDate == date)
        .ToListAsync();

    public Task<Attendance?> GetByPlayerAndDateAsync(int playerId, DateOnly date) => context.Attendances
        .FirstOrDefaultAsync(a => a.PlayerId == playerId && a.AttendanceDate == date);

    public async Task<PagedResult<Attendance>> GetHistoryAsync(PaginationRequest request,
        DateOnly start, DateOnly end)
    {
        int pageNumber = Math.Max(1, request.PageNumber);
        int pageSize = Math.Clamp(request.PageSize, 1, 100);

        IQueryable<Attendance> query = context.Attendances
        .AsNoTracking()
        .Include(a => a.Player)
        .Where(a => a.AttendanceDate >= start && a.AttendanceDate < end);

        var search = request.SearchTerm?.Trim();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(a => a.Player.Name.Contains(search)
                || (a.Status != null && a.Status.Contains(search))
                || (a.Remarks != null && a.Remarks.Contains(search)));

        var descending = request.SortOrder < 0;
        query = (request.SortField?.Trim().ToLowerInvariant()) switch
        {
            "playername" => descending ? query.OrderByDescending(a => a.Player.Name) : query.OrderBy(a => a.Player.Name),
            "attendancedate" or "date" => descending ? query.OrderByDescending(a => a.CreatedAT) : query.OrderBy(a => a.CreatedAT),
            "status" => descending ? query.OrderByDescending(a => a.Status) : query.OrderBy(a => a.Status),
            "remarks" => descending ? query.OrderByDescending(a => a.Remarks) : query.OrderBy(a => a.Remarks),
            _ => query.OrderByDescending(a => a.AttendanceDate).ThenByDescending(a => a.CreatedAT)
        };

        int totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Attendance>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

    }
    public Task<List<Attendance>> GetPlayerHistoryAsync(int playerId) => context.Attendances
        .AsNoTracking()
        .Where(a => a.PlayerId == playerId)
        .OrderByDescending(a => a.AttendanceDate)
        .ToListAsync();

    public Task<AttendancePlayerSummaryDto?> GetPlayerSummaryAsync(string playerName, DateOnly start, DateOnly end) =>
        context.Attendances
            .AsNoTracking()
            .Where(a => a.AttendanceDate >= start && a.AttendanceDate < end && a.Player.Name.Contains(playerName))
            .GroupBy(a => new { a.PlayerId, a.Player.Name })
            .Select(group => new AttendancePlayerSummaryDto
            {
                PlayerName = group.Key.Name,
                PresentCount = group.Count(a => a.Status == "Present"),
                AbsentCount = group.Count(a => a.Status == "Absent")
            })
            .OrderBy(summary => summary.PlayerName)
            .FirstOrDefaultAsync();

    public Task<Attendance?> GetByIdAsync(int id) => context.Attendances
        .FirstOrDefaultAsync(a => a.Id == id);

    public async Task AddAsync(Attendance attendance)
    {
        await context.Attendances.AddAsync(attendance);
    }

    public async Task SaveChangesAsync() => await context.SaveChangesAsync();
}
