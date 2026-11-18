
using backend.Data;
using backend.Models;
using backend.DTOs.Membership;
using backend.DTOs.Pagination;
using backend.Helper;
using backend.Repo_Interface;
using Microsoft.EntityFrameworkCore;

namespace backend.Repository;

public class MembershipRepository : IMembershipRepository
{
    private readonly AppDbContext _context;

    public MembershipRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Player?> GetPlayerByIdAsync(int playerId)
    {
        return _context.Players
            .FirstOrDefaultAsync(p =>
                p.Id == playerId &&
                p.IsActive &&
                !p.IsDeleted);
    }

    public Task<PlayerMembership?> GetLatestMembershipAsync(int playerId)
    {
        return _context.PlayerMemberships
            .AsNoTracking()
            .Where(m =>
                m.PlayerId == playerId &&
                m.Status != "Cancelled")
            .OrderByDescending(m => m.EndDate)
            .ThenByDescending(m => m.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<PlayerMembership> CreatePurchaseAsync(
        PlayerMembership membership,
        CashIn cashIn)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            await _context.PlayerMemberships.AddAsync(membership);

            await _context.SaveChangesAsync();

            cashIn.PlayerId = membership.PlayerId;
            cashIn.MembershipId = membership.Id;
            cashIn.Amount = membership.FinalAmount;

            await _context.CashIns.AddAsync(cashIn);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return membership;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<PagedResult<PlayerMembership>> GetByPlayerIdAsync(int playerId, PaginationRequest request)
    {
        var query = _context.PlayerMemberships
            .AsNoTracking()
            .Where(m => m.PlayerId == playerId)
            .OrderByDescending(m => m.StartDate);
        var page = Math.Max(1, request.PageNumber);
        var size = Math.Clamp(request.PageSize, 1, 100);
        var count = await query.CountAsync();
        return new PagedResult<PlayerMembership>
        {
            Items = await query.Skip((page - 1) * size).Take(size).ToListAsync(),
            TotalCount = count,
            PageNumber = page,
            PageSize = size
        };
    }

    public async Task<PagedResult<ExpiringMemberDto>> GetExpiringMembersAsync(DateOnly from, DateOnly through, PaginationRequest request)
    {
        var query = _context.CashIns
            .AsNoTracking()
            .Where(c => c.CoverageEndDate != null &&
                !_context.CashIns.Any(next => next.PlayerId == c.PlayerId && next.CoverageEndDate > c.CoverageEndDate) &&
                c.CoverageEndDate >= from && c.CoverageEndDate <= through)
            .Select(c => new ExpiringMemberDto
            {
                PlayerId = c.PlayerId,
                PlayerName = c.Player.Name,
                Contact = c.Player.Contact,
                ExpiryDate = c.CoverageEndDate,
                CoverageType = c.MembershipId == null ? "30-day fee" : "Subscription"
            })
            .OrderBy(x => x.ExpiryDate);
        var page = Math.Max(1, request.PageNumber);
        var size = Math.Clamp(request.PageSize, 1, 100);
        var count = await query.CountAsync();
        return new PagedResult<ExpiringMemberDto>
        {
            Items = await query.Skip((page - 1) * size).Take(size).ToListAsync(),
            TotalCount = count,
            PageNumber = page,
            PageSize = size
        };
    }
}
