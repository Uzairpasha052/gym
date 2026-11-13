using backend.Data;
using backend.DTOs.Pagination;
using backend.Helper;
using backend.Models;
using backend.Repo_Interface;
using Microsoft.EntityFrameworkCore;

namespace backend.Repository;

public class CashRepository(AppDbContext context) : ICashRepository
{
    public Task<Player?> GetPlayerByIdAsync(int playerId) => context.Players
        .FirstOrDefaultAsync(p => p.Id == playerId);

    public async Task<CashIn> CreateCashInAsync(CashIn cashIn)
    {
        await context.CashIns.AddAsync(cashIn);
        await context.SaveChangesAsync();
        return cashIn;
    }

    public async Task<CashOut> CreateCashOutAsync(CashOut cashOut)
    {
        await context.CashOuts.AddAsync(cashOut);
        await context.SaveChangesAsync();
        return cashOut;
    }

    public async Task<PagedResult<CashIn>> GetCashInsAsync(PaginationRequest request,
        DateTime start, DateTime end)
    {
        int pageNumber = Math.Max(1, request.PageNumber);
        int pageSize = Math.Clamp(request.PageSize, 1, 100);

        var query = context.CashIns
        .AsNoTracking()
        .Include(c => c.Player)
        .Where(c => c.PaymentDate >= start && c.PaymentDate < end)
        .OrderByDescending(c => c.PaymentDate);
        //.ToListAsync();

        int totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<CashIn>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

    }

    public async Task<PagedResult<CashOut>> GetCashOutsAsync(PaginationRequest request, 
        DateTime start, DateTime end)
    {
        int pageNumber = Math.Max(1, request.PageNumber);
        int pageSize = Math.Clamp(request.PageSize, 1, 100);

        var query = context.CashOuts
        .AsNoTracking()
        .Where(c => c.PaymentDate >= start && c.PaymentDate < end)
        .OrderByDescending(c => c.PaymentDate);
        //.ToListAsync();

        int totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<CashOut>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

}
