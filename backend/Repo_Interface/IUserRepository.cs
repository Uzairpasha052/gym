using backend.DTOs.Pagination;
using backend.Helper;
using backend.Model.Database;
using backend.Models;

namespace backend.Repo_Interface;

public interface IUserRepository
{
    Task<Role?> GetRoleByIdAsync(int roleId);
    Task<User?> GetByEmailAsync(string email);
    Task<PagedResult<User>> GetAllAsync(PaginationRequest request);
    Task<User?> GetByIdAsync(int id);
    Task<User> CreateAsync(User user);
    Task<bool> UpdateAsync(User user);
    Task<bool> DeleteAsync(User user);
}
