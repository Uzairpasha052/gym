using backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.DTO.Attendance;
using backend.DTOs.Pagination;

namespace backend.Controllers;

[Route("api/attendance")]
[ApiController]
[Authorize]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;

    public AttendanceController(IAttendanceService attendanceService)
    {
        _attendanceService = attendanceService;
    }

    // Load today's attendance page
    [HttpGet("today")]
    public async Task<IActionResult> GetTodayAttendance([FromQuery] PaginationRequest request)
    {
        var result = await _attendanceService.GetTodayAttendanceAsync(request);

        return Ok(result);
    }

    // Save today's attendance
    [HttpPost("save")]
    public async Task<IActionResult> SaveAttendance([FromBody] List<MarkAttendanceDto> dto)
    {
        var result = await _attendanceService.SaveAttendanceAsync(dto);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }


    // Attendance history for a calendar month (defaults to current month)
    [HttpGet("history")]
    public async Task<IActionResult> GetAttendanceHistory(
        [FromQuery] int? year,
        [FromQuery] int? month,
        [FromQuery] PaginationRequest request)
    {
        var result = await _attendanceService.GetAttendanceHistoryAsync(request, year, month);

        return Ok(result);
    }

    // Individual player attendance history
    [HttpGet("player/{playerId}")]
    public async Task<IActionResult> GetPlayerAttendanceHistory(int playerId)
    {
        var result = await _attendanceService.GetPlayerAttendanceHistoryAsync(playerId);

        return Ok(result);
    }

    // Update attendance (Admin only)
    [Authorize(Roles = "Admin")]
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateAttendance(
        int id, [FromBody] UpdateAttendanceDto dto)
    {
        var result = await _attendanceService.UpdateAttendanceAsync(id, dto);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

}
