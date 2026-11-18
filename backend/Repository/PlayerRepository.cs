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

        IQueryable<Player> query = context.Players.AsNoTracking();
        var search = request.SearchTerm?.Trim();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.Contains(search) || p.FName.Contains(search) || p.Age.Contains(search)
                || p.Gender.Contains(search) || p.Contact.Contains(search) || p.Fee.Contains(search) || p.Status.Contains(search));

        var descending = request.SortOrder < 0;
        query = (request.SortField?.Trim().ToLowerInvariant()) switch
        {
            "name" => descending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
            "fname" or "fathername" => descending ? query.OrderByDescending(p => p.FName) : query.OrderBy(p => p.FName),
            "age" => descending ? query.OrderByDescending(p => p.Age) : query.OrderBy(p => p.Age),
            "gender" => descending ? query.OrderByDescending(p => p.Gender) : query.OrderBy(p => p.Gender),
            "contact" => descending ? query.OrderByDescending(p => p.Contact) : query.OrderBy(p => p.Contact),
            "fee" => descending ? query.OrderByDescending(p => p.Fee) : query.OrderBy(p => p.Fee),
            "status" => descending ? query.OrderByDescending(p => p.Status) : query.OrderBy(p => p.Status),
            "createdat" => descending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
            _ => descending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name)
        };

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
