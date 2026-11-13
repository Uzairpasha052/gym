using backend.Model.Database;

namespace backend.Repo_Interface;

public interface IRoleRepository
{
    Task<List<Role>> GetAllAsync();
    Task<Role?> GetByIdAsync(int id);
    Task<bool> NameExistsAsync(string name, int? exceptId = null);
    Task<bool> IsAssignedAsync(int id);
    Task AddAsync(Role role);
    Task DeleteAsync(Role role);
    Task SaveChangesAsync();
}
