using backend.Data;
using backend.DTOs.Pagination;
using backend.Helper;
using backend.Model.Database;
using backend.Models;
using backend.Repo_Interface;
using Microsoft.EntityFrameworkCore;

namespace backend.Repository;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public Task<Role?> GetRoleByIdAsync(int roleId) => context.Roles.FirstOrDefaultAsync(r => r.Id == roleId);

    public Task<User?> GetByEmailAsync(string email) => context.Users
        .Include(u => u.Role)
        .FirstOrDefaultAsync(u => u.Email == email);

    public async Task<PagedResult<User>> GetAllAsync(PaginationRequest request)
    {
        int pageNumber = Math.Max(1, request.PageNumber);
        int pageSize = Math.Clamp(request.PageSize, 1, 100);

        var query = context.Users
        .AsNoTracking()
        .Include(u => u.Role)
        .OrderBy(u => u.Id);
        //.ToListAsync();

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
