using backend.Helper;
using backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController(IDashboardService dashboard, CurrentUserService currentUser) : ControllerBase
{
    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Admin() => Ok(await dashboard.GetAdminAsync());

    [HttpGet("receptionist")]
    [Authorize(Roles = "Receptionist")]
    public async Task<IActionResult> Receptionist() => Ok(await dashboard.GetReceptionistAsync());

    [HttpGet("member")]
    [Authorize(Roles = "Member")]
    public async Task<IActionResult> Member()
    {
        if (currentUser.UserId is not int userId) return Unauthorized();
        var result = await dashboard.GetMemberAsync(userId);
        return result is null
            ? Conflict(new { message = "Your member account is not linked to a player profile. Contact the gym administrator." })
            : Ok(result);
    }
}
