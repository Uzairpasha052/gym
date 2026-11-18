
using backend.DTOs.Membership;
using backend.DTOs.Pagination;
using backend.Helper;
using backend.Interfaces;
using backend.Models;
using backend.Repo_Interface;
using System.Globalization;
using System.Text.Json;

namespace backend.Services;

public class MembershipService : IMembershipService
{
    private readonly IMembershipRepository _repository;
    private readonly CurrentUserService _currentUser;
    private readonly IAuditLogService _auditLog;

    private const decimal MembershipDiscount = 1000m;

    public MembershipService(
        IMembershipRepository repository,
        CurrentUserService currentUser,
        IAuditLogService auditLog)
    {
        _repository = repository;
        _currentUser = currentUser;
        _auditLog = auditLog;
    }

    public async Task<MembershipResponseDto> PurchaseMembershipAsync(
        CreateMembershipDto dto)
    {
        if (dto.PlayerId <= 0)
            throw new ArgumentException("Invalid player ID.");

        if (dto.DurationMonths < 3)
            throw new ArgumentException(
                "Membership duration must be at least 3 months.");

        var player = await _repository.GetPlayerByIdAsync(dto.PlayerId);

        if (player == null)
            throw new KeyNotFoundException("Player not found.");

        // Read monthly fee from existing Player.Fee
        if (!decimal.TryParse(
            player.Fee,
            NumberStyles.Number,
            CultureInfo.InvariantCulture,
            out decimal monthlyFee) || monthlyFee <= 0)
        {
            throw new ArgumentException(
                "Player monthly fee is invalid.");
        }

        // Calculate membership amount
        decimal originalAmount = monthlyFee * dto.DurationMonths;

        decimal discountAmount = MembershipDiscount;

        decimal finalAmount = originalAmount - discountAmount;

        if (finalAmount <= 0)
        {
            throw new ArgumentException(
                "Membership fee must be greater than the discount.");
        }

        // Calculate membership dates
        var today = DateOnly.FromDateTime(DateTime.Today);

        var latestMembership =
            await _repository.GetLatestMembershipAsync(dto.PlayerId);

        DateOnly startDate = today;

        if (latestMembership != null &&
            latestMembership.EndDate >= today)
        {
            startDate = latestMembership.EndDate.AddDays(1);
        }

        DateOnly endDate = startDate
            .AddMonths(dto.DurationMonths)
            .AddDays(-1);

        string status = startDate > today
            ? "Scheduled"
            : "Active";

        var membership = new PlayerMembership
        {
            PlayerId = player.Id,
            DurationMonths = dto.DurationMonths,

            StartDate = startDate,
            EndDate = endDate,

            MonthlyFee = monthlyFee,

            OriginalAmount = originalAmount,
            DiscountAmount = discountAmount,
            FinalAmount = finalAmount,

            Status = status,

            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUser.UserId
        };

        // Full one-time payment
        var cashIn = new CashIn
        {
            PlayerId = player.Id,
            Amount = finalAmount,
            PaymentDate = DateTime.UtcNow,
            CoverageStartDate = startDate,
            CoverageEndDate = endDate,
            CreatedBy = _currentUser.UserId
        };

        // Save membership and CashIn together
        var result = await _repository.CreatePurchaseAsync(
            membership,
            cashIn);

        // Audit logging
        var newValues = JsonSerializer.Serialize(new
        {
            result.Id,
            PlayerName = player.Name,
            result.DurationMonths,
            result.StartDate,
            result.EndDate,
            result.MonthlyFee,
            result.OriginalAmount,
            result.DiscountAmount,
            result.FinalAmount,
            result.Status,
            CashInId = cashIn.Id
        });

        await _auditLog.LogAsync(
            _currentUser.UserId,
            "Create",
            "PlayerMembership",
            result.Id.ToString(),
            null,
            newValues
        );

        return MapToResponse(result, player.Name);
    }


    public async Task<PagedResult<MembershipListDto>> GetPlayerMembershipsAsync(int playerId, PaginationRequest request)
    {
        var memberships =
            await _repository.GetByPlayerIdAsync(playerId, request);

        return new PagedResult<MembershipListDto>
        {
            Items = memberships.Items.Select(m => new MembershipListDto
            {
                Id = m.Id,
                DurationMonths = m.DurationMonths,
                StartDate = m.StartDate,
                EndDate = m.EndDate,
                FinalAmount = m.FinalAmount,
                DiscountAmount = m.DiscountAmount,
                Status = m.Status
            }).ToList(),
            TotalCount = memberships.TotalCount,
            PageNumber = memberships.PageNumber,
            PageSize = memberships.PageSize
        };
    }

    public Task<PagedResult<ExpiringMemberDto>> GetExpiringMembersAsync(int days, PaginationRequest request)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        return _repository.GetExpiringMembersAsync(today, today.AddDays(Math.Clamp(days, 0, 90)), request);
    }

    private static MembershipResponseDto MapToResponse(
        PlayerMembership membership,
        string playerName)
    {
        return new MembershipResponseDto
        {
            Id = membership.Id,
            PlayerId = membership.PlayerId,
            PlayerName = playerName,

            DurationMonths = membership.DurationMonths,

            StartDate = membership.StartDate,
            EndDate = membership.EndDate,

            MonthlyFee = membership.MonthlyFee,

            OriginalAmount = membership.OriginalAmount,
            DiscountAmount = membership.DiscountAmount,
            FinalAmount = membership.FinalAmount,

            Status = membership.Status,

            CashInId = membership.CashIn?.Id,

            CreatedAt = membership.CreatedAt
        };
    }

}
