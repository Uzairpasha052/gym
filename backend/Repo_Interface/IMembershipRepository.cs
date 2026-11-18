
using backend.Models;
using backend.DTOs.Membership;
using backend.DTOs.Pagination;
using backend.Helper;

namespace backend.Repo_Interface;

public interface IMembershipRepository
{
    Task<Player?> GetPlayerByIdAsync(int playerId);

    Task<PlayerMembership?> GetLatestMembershipAsync(int playerId);

    Task<PlayerMembership> CreatePurchaseAsync(
        PlayerMembership membership,
        CashIn cashIn);

    Task<PagedResult<PlayerMembership>> GetByPlayerIdAsync(int playerId, PaginationRequest request);
    Task<PagedResult<ExpiringMemberDto>> GetExpiringMembersAsync(DateOnly from, DateOnly through, PaginationRequest request);
}
