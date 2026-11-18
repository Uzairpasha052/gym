using backend.Data;
using backend.Models;
using backend.Repo_Interface;
using Microsoft.EntityFrameworkCore;

namespace backend.Repository;

public class RoleRepository : IRoleRepository
{
    private readonly AppDbContext _context;
    public RoleRepository(AppDbContext context) => _context = context;

    public Task<List<Role>> GetAllAsync() 
    { 
        return _context.Roles
            .AsNoTracking()
            .Where(u => u.IsActive == true)
            .OrderBy(r => r.Name)
            .ToListAsync(); 
    }
    
    public Task<Role?> GetByIdAsync(int id) => _context.Roles.FirstOrDefaultAsync(r => r.Id == id);
    
    public Task<bool> NameExistsAsync(string name, int? exceptId = null) =>
        _context.Roles.AnyAsync(r => r.Name.ToLower() == name.ToLower() && (!exceptId.HasValue || r.Id != exceptId.Value));
    
    public Task<bool> IsAssignedAsync(int id) => _context.Users.AnyAsync(u => u.RoleId == id);

    public async Task AddAsync(Role role) => await _context.Roles.AddAsync(role);
    
    public Task DeleteAsync(Role role) 
    {
        _context.Roles.Update(role); 
        return Task.CompletedTask; 
    }
    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
