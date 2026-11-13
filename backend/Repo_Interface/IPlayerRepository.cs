using backend.DTOs.Pagination;
using backend.Helper;
using backend.Models;

namespace backend.Repo_Interface;

public interface IPlayerRepository
{
    Task<PagedResult<Player>> GetAllAsync(PaginationRequest request);
    Task<Player?> GetByIdAsync(int id);
    Task<Player> CreateAsync(Player player);
    Task<bool> UpdateAsync(Player player);
}
