using backend.DTO.Attendance;
using backend.DTOs;
using backend.DTOs.Pagination;
using backend.Helper;
using backend.Interfaces;
using backend.Models;
using backend.Repo_Interface;
using System.Text.Json;

namespace backend.Services;

public class AttendanceService : IAttendanceService
{
    private readonly IAttendanceRepository _repository;
    private readonly CurrentUserService _currentUser;
    private readonly IAuditLogService _logService;


    public AttendanceService(IAttendanceRepository repository, CurrentUserService currentUser,
        IAuditLogService logService)
    { 
        _repository = repository; 
        _currentUser = currentUser;
        _logService = logService;
    }

    public async Task<PagedResult<TodayAttendanceDto>> GetTodayAttendanceAsync(PaginationRequest request)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var players = await _repository.GetActivePlayersAsync(request);
        var attendances = await _repository.GetByDateAsync(today);

        return new PagedResult<TodayAttendanceDto>
        {
            Items = players.Items.Select(p =>
            {
                var attendance = attendances.FirstOrDefault(a => a.PlayerId == p.Id);
                return new TodayAttendanceDto
                {
                    PlayerId = p.Id,
                    PlayerName = p.Name,
                    AttendanceDate = today,
                    Status = attendance?.Status ?? "Absent"
                };
            }).ToList(),

            TotalCount = players.TotalCount,
            PageNumber = players.PageNumber,
            PageSize = players.PageSize
        };

        //return 
    }

    public async Task<AttendanceResponseDto> SaveAttendanceAsync(List<MarkAttendanceDto> dto)
    {
        if (dto is null || dto.Count == 0)
            return new AttendanceResponseDto { Success = false, Message = "No attendance data received." };

        var today = DateOnly.FromDateTime(DateTime.Today);
        var pendingLogs = new List<(Attendance Attendance, string Action, string? OldValues)>();
        foreach (var item in dto)
        {
            var attendance = await _repository.GetByPlayerAndDateAsync(item.PlayerId, today);
            if (attendance is null)
            {
                attendance = new Attendance
                {
                    PlayerId = item.PlayerId,
                    AttendanceDate = today,
                    Status = item.Status,
                    Remarks = item.Remarks,
                    CreatedAT = DateTime.UtcNow,
                    CreatedBy = _currentUser.UserId
                };
                await _repository.AddAsync(attendance);
                pendingLogs.Add((attendance, "Create", null));
            }
            else
            {
                var oldValues = JsonSerializer.Serialize(new
                {
                    attendance.PlayerId,
                    attendance.AttendanceDate,
                    attendance.Status,
                    attendance.Remarks
                });
                attendance.Status = item.Status;
                attendance.Remarks = item.Remarks;
                attendance.UpdatedAt = DateTime.UtcNow;
                attendance.UpdatedBy = _currentUser.UserId;
                pendingLogs.Add((attendance, "Update", oldValues));
            }
        }

        await _repository.SaveChangesAsync();

        foreach (var pending in pendingLogs)
        {
            var attendance = pending.Attendance;
            var newValues = JsonSerializer.Serialize(new
            {
                attendance.PlayerId,
                attendance.AttendanceDate,
                attendance.Status,
                attendance.Remarks
            });

            await _logService.LogAsync(
                _currentUser.UserId,
                pending.Action,
                "Attendance",
                attendance.Id.ToString(),
                pending.OldValues,
                newValues
            );
        }

        return new AttendanceResponseDto { Success = true, Message = "Attendance saved successfully." };
    }

    public async Task<PagedResult<AdminAttendanceDto>> GetAttendanceHistoryAsync(PaginationRequest request,
        int? year = null, int? month = null)
    {
        var start = new DateOnly(year ?? DateTime.Today.Year, month ?? DateTime.Today.Month, 1);
        var attendances = await _repository.GetHistoryAsync(request, start, start.AddMonths(1));

        return new PagedResult<AdminAttendanceDto>
        {
            Items = attendances.Items.Select(a => new AdminAttendanceDto
            {
                AttendanceId = a.Id,
                PlayerId = a.PlayerId,
                PlayerName = a.Player.Name,
                // CreatedAT is stored in UTC. Returning the timestamp (instead of
                // the date-only column) lets clients display the time it was marked.
                AttendanceDate = a.CreatedAT,
                Status = a.Status ?? string.Empty,
                Remarks = a.Remarks
            }).ToList(),

            TotalCount = attendances.TotalCount,
            PageNumber = attendances.PageNumber,
            PageSize = attendances.PageSize

        };
 
    }

    public async Task<List<AttendanceHistoryDto>> GetPlayerAttendanceHistoryAsync(int playerId)
    {
        var attendances = await _repository.GetPlayerHistoryAsync(playerId);
        return attendances.Select(a => new AttendanceHistoryDto
        {
            AttendanceDate = a.AttendanceDate,
            Status = a.Status
        }).ToList();
    }

    public async Task<AttendanceResponseDto> UpdateAttendanceAsync(int attendanceId, UpdateAttendanceDto dto)
    {
        var attendance = await _repository.GetByIdAsync(attendanceId);
        if (attendance is null)
            return new AttendanceResponseDto 
            { 
                Success = false, 
                Message = "Attendance record not found." 
            };

        var oldValues = JsonSerializer.Serialize(new
        {
            //attendance.Id,
            attendance.PlayerId,
            attendance.AttendanceDate,
            attendance.Status,
            attendance.Remarks
        });


        if (!string.IsNullOrWhiteSpace(dto.Status)) attendance.Status = dto.Status;
        if (dto.Remarks is not null) attendance.Remarks = dto.Remarks;

        attendance.UpdatedAt = DateTime.UtcNow;
        attendance.UpdatedBy = _currentUser.UserId;

        var newValues = JsonSerializer.Serialize(new
        {
            //attendance.Id,
            attendance.PlayerId,
            attendance.AttendanceDate,
            attendance.Status,
            attendance.Remarks
        });

        await _logService.LogAsync(
            _currentUser.UserId,
            "Update",
            "Attendance",
            attendance.Id.ToString(),
            oldValues,
            newValues
        );

        await _repository.SaveChangesAsync();
        return new AttendanceResponseDto
        {
            Success = true,
            Message = "Attendance updated successfully.",
            AttendanceId = attendance.Id,
            Status = attendance.Status ?? string.Empty
        };
    }
}
