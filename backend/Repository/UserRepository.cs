using backend.Data;
using backend.DTOs.Pagination;
using backend.Helper;
using backend.Models;
using backend.Repo_Interface;
using Microsoft.EntityFrameworkCore;

namespace backend.Repository;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public Task<Role?> GetRoleByIdAsync(int roleId) => context.Roles.FirstOrDefaultAsync(r => r.Id == roleId);

    public Task<Player?> GetPlayerByIdAsync(int playerId) => context.Players
        .FirstOrDefaultAsync(p => p.Id == playerId && p.IsActive && !p.IsDeleted);

    public Task<bool> IsPlayerLinkedAsync(int playerId, int? exceptUserId = null) => context.Users
        .AnyAsync(u => u.PlayerId == playerId && (!exceptUserId.HasValue || u.Id != exceptUserId.Value));

    public Task<User?> GetByEmailAsync(string email) => context.Users
        .Include(u => u.Role)
        .Include(u => u.Player)
        .FirstOrDefaultAsync(u => u.Email == email);

    public async Task<PagedResult<User>> GetAllAsync(PaginationRequest request)
    {
        int pageNumber = Math.Max(1, request.PageNumber);
        int pageSize = Math.Clamp(request.PageSize, 1, 100);

        IQueryable<User> query = context.Users
        .AsNoTracking()
        //.Where(u => u.IsActive)
        .Include(u => u.Role)
        .Include(u => u.Player);

        var search = request.SearchTerm?.Trim();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(u => u.FullName.Contains(search) || u.Email.Contains(search) || u.Role.Name.Contains(search));

        var descending = request.SortOrder < 0;
        query = (request.SortField?.Trim().ToLowerInvariant()) switch
        {
            "fullname" or "name" => descending ? query.OrderByDescending(u => u.FullName) : query.OrderBy(u => u.FullName),
            "email" => descending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
            "rolename" or "role" => descending ? query.OrderByDescending(u => u.Role.Name) : query.OrderBy(u => u.Role.Name),
            "createdat" => descending ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt),
            _ => descending ? query.OrderByDescending(u => u.Id) : query.OrderBy(u => u.Id)
        };

        int totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<User>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public Task<User?> GetByIdAsync(int id) => context.Users
        .Include(u => u.Role)
        .Include(u => u.Player)
        .FirstOrDefaultAsync(u => u.Id == id);

    public async Task<User> CreateAsync(User user)
    {
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> UpdateAsync(User user)
    {
        context.Users.Update(user);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(User user)
    {
        context.Users.Remove(user);
        return await context.SaveChangesAsync() > 0;
    }
}
