using backend.Data;
using backend.DTOs.Pagination;
using backend.Helper;
using backend.Models;
using backend.Repo_Interface;
using Microsoft.EntityFrameworkCore;

namespace backend.Repository;

public class PlayerRepository(AppDbContext context) : IPlayerRepository
{
    public async Task<PagedResult<Player>> GetAllAsync(PaginationRequest request)
    {
        int pageNumber = Math.Max(1, request.PageNumber);
        int pageSize = Math.Clamp(request.PageSize, 1, 100);

        var query = context.Players
        .AsNoTracking()
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

    public Task<Player?> GetByIdAsync(int id) => context.Players
        .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Player> CreateAsync(Player player)
    {
        await context.Players.AddAsync(player);
        await context.SaveChangesAsync();
        return player;
    }

    public async Task<bool> UpdateAsync(Player player)
    {
        context.Players.Update(player);
        return await context.SaveChangesAsync() > 0;
    }
}
