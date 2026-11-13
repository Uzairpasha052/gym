using backend.DTO.Reports;
using backend.DTOs.Reports;
using backend.Interfaces;
using backend.Repo_Interface;
using backend.Services.Interfaces;

namespace backend.Services;

public class ReportService : IReportService
{
    private readonly IReportRepository _repository;
    private readonly IAuditLogService _logService;

    public ReportService(IReportRepository repository, IAuditLogService logService)
    {
        _repository = repository;
        _logService = logService;
    }

    public async Task<List<PlayerRecordReportDto>> GetPlayerReportAsync(string status)
    {
        var players = await _repository.GetPlayersAsync();
        if (string.Equals(status, "active", StringComparison.OrdinalIgnoreCase))
            players = players.Where(p => string.Equals(p.Status, "active", StringComparison.OrdinalIgnoreCase)).ToList();
        else if (string.Equals(status, "inactive", StringComparison.OrdinalIgnoreCase))
            players = players.Where(p => string.Equals(p.Status, "inactive", StringComparison.OrdinalIgnoreCase)).ToList();

        return players.Select(p => new PlayerRecordReportDto
        {
            Id = p.Id,
            Name = p.Name,
            FName = p.FName,
            Age = p.Age,
            Gender = p.Gender,
            Contact = p.Contact,
            History = p.History,
            Fee = p.Fee,
            Status = p.Status,
            CreatedAt = p.CreatedAt
        }).ToList();
    }

    public async Task<List<PlayerReasonReportDto>> GetPlayerReasonReportAsync()
    {
        var players = await _repository.GetPlayersAsync();
        return players.Where(p => !string.IsNullOrWhiteSpace(p.Reason)).Select(p => new PlayerReasonReportDto
        {
            Id = p.Id,
            Name = p.Name,
            FName = p.FName,
            Age = p.Age,
            Gender = p.Gender,
            Reason = p.Reason
        }).ToList();
    }

    public async Task<List<AttendanceReportDto>> GetAttendanceReportAsync(DateOnly StartDate, DateOnly EndDate)
    {
        var attendances = await _repository.GetAttendanceAsync(StartDate, EndDate);
        return attendances.Select(a => new AttendanceReportDto
        {
            PlayerName = a.Player.Name,
            FatherName = a.Player.FName,
            AttendanceDate = a.AttendanceDate,
            Status = a.Status ?? string.Empty,
            Remarks = a.Remarks ?? string.Empty
        }).ToList();
    }

    public async Task<List<CashInReportDto>> GetCashInReportAsync(DateOnly startDate, DateOnly endDate)
    {
        var start = startDate.ToDateTime(TimeOnly.MinValue);
        var end = endDate.ToDateTime(TimeOnly.MaxValue);
        var cashIns = await _repository.GetCashInsAsync(start, end);
        return cashIns.Select(c => new CashInReportDto
        {
            PlayerName = c.Player.Name,
            FatherName = c.Player.FName,
            Amount = c.Amount,
            PaymentDate = DateOnly.FromDateTime(c.PaymentDate)
        }).ToList();
    }

    public async Task<List<CashOutReportDto>> GetCashOutReportAsync(DateOnly startDate, DateOnly endDate)
    {
        var cashOuts = await _repository.GetCashOutsAsync(
            startDate.ToDateTime(TimeOnly.MinValue), endDate.ToDateTime(TimeOnly.MaxValue));
        return cashOuts.Select(c => new CashOutReportDto
        {
            Amount = c.Amount,
            ExpenseDate = DateOnly.FromDateTime(c.PaymentDate),
            Description = c.Description
        }).ToList();
    }

    public async Task<CashFlowReportDto> GetCashFlowReportAsync(DateOnly startDate, DateOnly endDate)
    {
        var start = startDate.ToDateTime(TimeOnly.MinValue);
        var end = endDate.ToDateTime(TimeOnly.MaxValue);
        var cashIns = await _repository.GetCashInsAsync(start, end);
        var cashOuts = await _repository.GetCashOutsAsync(start, end);
        var cashInDtos = cashIns.Select(c => new CashFlowCashInDto
        {
            PlayerName = c.Player.Name,
            FatherName = c.Player.FName,
            Amount = c.Amount,
            PaymentDate = DateOnly.FromDateTime(c.PaymentDate)
        }).ToList();
        var cashOutDtos = cashOuts.Select(c => new CashFlowCashOutDto
        {
            Amount = c.Amount,
            Description = c.Description,
            ExpenseDate = DateOnly.FromDateTime(c.PaymentDate)
        }).ToList();

        var totalCashIn = cashInDtos.Sum(c => c.Amount);
        var totalCashOut = cashOutDtos.Sum(c => c.Amount);
        return new CashFlowReportDto
        {
            StartDate = startDate,
            EndDate = endDate,
            CashIns = cashInDtos,
            CashOuts = cashOutDtos,
            TotalCashIn = totalCashIn,
            TotalCashOut = totalCashOut,
            ProfitAmount = totalCashIn - totalCashOut
        };
    }
}
