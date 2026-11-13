using backend.DTOs.Pagination;
using backend.DTOs.Player;
using backend.Helper;

namespace backend.Interfaces;

public interface IPlayerService
{
    Task<PagedResult<PlayerResponseDto>> GetAllPlayersAsync(PaginationRequest request);

    Task<PlayerResponseDto?> GetPlayerByIdAsync(int id);

    Task<PlayerResponseDto> CreatePlayerAsync(CreatePlayerDto dto);

    Task<PlayerResponseDto?> UpdatePlayerAsync(int id, UpdatePlayerDto dto);

    Task<bool> DeletePlayerAsync(int id);
}