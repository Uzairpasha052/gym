using backend.DTO.CashIn;
using backend.DTO.CashOut;
using backend.DTOs.Pagination;
using backend.Helper;
using backend.Interfaces;
using backend.Models;
using backend.Repo_Interface;
using System.Text.Json;

namespace backend.Services;

public class CashService : ICashService
{
    private readonly ICashRepository _repository;
    private readonly CurrentUserService _currentUser;
    private readonly IAuditLogService _logService;


    public CashService(ICashRepository repository, CurrentUserService currentUser, 
        IAuditLogService logService)
    {
        _repository = repository;
        _currentUser = currentUser;
        _logService = logService;
    }

    public async Task<CashInResponseDto> CashInAsync(CashInDto dto)
    {
        var player = await _repository.GetPlayerByIdAsync(dto.PlayerId);
        if (player is null)
            return new CashInResponseDto { Success = false, Message = "Player not found." };

        if (dto.Amount <= 0)
            return new CashInResponseDto { Success = false, Message = "Amount must be greater than zero." };

        var paymentDate = dto.PaymentDate == default ? DateTime.UtcNow : dto.PaymentDate;
        var today = DateOnly.FromDateTime(DateTime.Today);
        var latestCoverageEnd = await _repository.GetLatestCoverageEndAsync(dto.PlayerId);

        // Allow renewal during the last 7 days of the current coverage period.
        // This check uses today's date rather than the client-supplied payment date
        // so a backdated or future-dated request cannot bypass the renewal window.
        if (latestCoverageEnd.HasValue && latestCoverageEnd.Value > today.AddDays(7))
        {
            return new CashInResponseDto
            {
                Success = false,
                Message = $"This player has already paid. Another cash-in is allowed starting {latestCoverageEnd.Value.AddDays(-7):dd MMM yyyy}."
            };
        }

        var coverageStart = latestCoverageEnd.HasValue && latestCoverageEnd.Value >= today
            ? latestCoverageEnd.Value.AddDays(1)
            : today;

        var cashIn = new CashIn
        {
            PlayerId = dto.PlayerId,
            Player = player,
            Amount = dto.Amount,
            PaymentDate = paymentDate,
            CoverageStartDate = coverageStart,
            CoverageEndDate = coverageStart.AddDays(29),
            CreatedBy = _currentUser.UserId
        };
        
        var result = await _repository.CreateCashInAsync(cashIn);

        var newValues = JsonSerializer.Serialize(new
        {
            result.Id,
            PlayerName = player.Name,
            result.Amount,
            result.PaymentDate,
            result.CoverageStartDate,
            result.CoverageEndDate,
        });

        await _logService.LogAsync(
            _currentUser.UserId,
            "Create",
            "CashIn",
            result.Id.ToString(),
            null,
            newValues
        );

        return new CashInResponseDto { Success = true, Message = "Cash in saved successfully." };
    }

    public async Task<bool> CashOutAsync(CashOutDto dto)
    {
        if (dto.Amount <= 0 || string.IsNullOrWhiteSpace(dto.Description)) return false;

        var cashOut = new CashOut
        {
            Amount = dto.Amount,
            PaymentDate = dto.PaymentDate == default ? DateTime.UtcNow : dto.PaymentDate,
            Description = dto.Description.Trim(),
            CreatedBy = _currentUser.UserId
        };

        var result = await _repository.CreateCashOutAsync(cashOut);

        var newValues = JsonSerializer.Serialize(new
        {
            result.Id,
            result.Description,
            result.Amount,
            result.PaymentDate,
        });

        await _logService.LogAsync(
            _currentUser.UserId,
            "Create",
            "CashOut",
            result.Id.ToString(),
            null,
            newValues
        );

        return true;
    }

    public async Task<PagedResult<CashInListDto>> GetAllCashInAsync(PaginationRequest request,
        int? year = null, int? month = null)
    {
        var (start, end) = GetMonthRange(year, month);
        var cashIns = await _repository.GetCashInsAsync(request, start, end);

        return new PagedResult<CashInListDto>
        {
            Items = cashIns.Items.Select(c => new CashInListDto
            {
                Id = c.Id,
                PlayerId = c.PlayerId,
                PlayerName = c.Player.Name,
                Amount = c.Amount,
                PaymentDate = c.PaymentDate,
                CoverageStartDate = c.CoverageStartDate,
                CoverageEndDate = c.CoverageEndDate
            }).ToList(),

            TotalCount = cashIns.TotalCount,
            PageNumber = cashIns.PageNumber,
            PageSize = cashIns.PageSize
        };
        //return 
    }

    public async Task<PagedResult<CashOutDto>> GetAllCashOutAsync(PaginationRequest request, 
        int? year = null, int? month = null)
    {
        var (start, end) = GetMonthRange(year, month);
        var cashOuts = await _repository.GetCashOutsAsync(request, start, end);

        return new PagedResult<CashOutDto>
        {
            Items = cashOuts.Items.Select(c => new CashOutDto
            {
                Id = c.Id,
                Amount = c.Amount,
                Description = c.Description,
                PaymentDate = c.PaymentDate
            }).ToList(),

            TotalCount = cashOuts.TotalCount,
            PageNumber = cashOuts.PageNumber,
            PageSize = cashOuts.PageSize
        };
        //return 
    }

    private static (DateTime Start, DateTime End) GetMonthRange(int? year, int? month)
    {
        var start = new DateTime(year ?? DateTime.Today.Year, month ?? DateTime.Today.Month, 1);
        return (start, start.AddMonths(1));
    }
}
