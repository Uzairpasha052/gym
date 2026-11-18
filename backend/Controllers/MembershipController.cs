
using backend.DTOs.Membership;
using backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.DTOs.Pagination;

namespace backend.Controllers;

[Route("api/membership")]
[ApiController]
[Authorize]
public class MembershipController : ControllerBase
{
    private readonly IMembershipService _membershipService;

    public MembershipController(IMembershipService membershipService)
    {
        _membershipService = membershipService;
    }

    [HttpPost("purchase")]
    public async Task<IActionResult> PurchaseMembership(
        [FromBody] CreateMembershipDto dto)
    {
        try
        {
            var result =
                await _membershipService.PurchaseMembershipAsync(dto);

            return Ok(new
            {
                message = "Membership purchased successfully.",
                data = result
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("player/{playerId:int}")]
    public async Task<IActionResult> GetPlayerMemberships(
        int playerId, [FromQuery] PaginationRequest request)
    {
        var result =
            await _membershipService.GetPlayerMembershipsAsync(playerId, request);

        return Ok(result);
    }

    [HttpGet("expiring")]
    public async Task<IActionResult> GetExpiringMembers([FromQuery] int days = 7, [FromQuery] PaginationRequest? request = null)
    {
        var result = await _membershipService.GetExpiringMembersAsync(days, request ?? new PaginationRequest());
        return Ok(result);
    }
}
