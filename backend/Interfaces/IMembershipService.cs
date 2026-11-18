
using backend.DTOs.Membership;
using backend.DTOs.Pagination;
using backend.Helper;

namespace backend.Interfaces;

public interface IMembershipService
{
    Task<MembershipResponseDto> PurchaseMembershipAsync(
        CreateMembershipDto dto);

    Task<PagedResult<MembershipListDto>> GetPlayerMembershipsAsync(int playerId, PaginationRequest request);
    Task<PagedResult<ExpiringMemberDto>> GetExpiringMembersAsync(int days, PaginationRequest request);

}
