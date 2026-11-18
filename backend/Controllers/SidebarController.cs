
using backend.DTOs.SideBar;
using backend.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SidebarController : ControllerBase
{
    private readonly ISidebarService _service;

    public SidebarController(ISidebarService sidebarService)
    {
        _service = sidebarService;
    }

    [HttpGet("parents")]
    public async Task<IActionResult> GetParentMenus()
    {
        var roleIdClaim = User.FindFirst("roleId")?.Value;

        if (!int.TryParse(roleIdClaim, out int roleId))
        {
            return Unauthorized(new
            {
                message = "Role information is missing from token."
            });
        }

        var menus = await _service.GetParentMenusAsync(roleId);

        return Ok(menus);
    }


    [HttpGet("children/{parentId:int}")]
    public async Task<IActionResult> GetChildMenus(int parentId)
    {
        var roleIdClaim = User.FindFirst("roleId")?.Value;

        if (!int.TryParse(roleIdClaim, out int roleId))
        {
            return Unauthorized(new
            {
                message = "Role information is missing from token."
            });
        }

        var menus = await _service.GetChildMenusAsync(roleId, parentId);

        return Ok(menus);
    }


    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(int id)
    {
        var menu = await _service.GetByIdAsync(id);

        if (menu == null)
            return NotFound(new { message = "Sidebar menu not found." });

        return Ok(menu);
    }


    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(
        [FromBody] CreateSidebarMenuDto dto)
    {
        var result = await _service.CreateAsync(dto);

        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            new
            {
                id = result.Id,
                message = result.Message
            });
    }


    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateSidebarMenuDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);

        if (!result.Success)
        {
            if (result.Message == "Sidebar menu not found.")
                return NotFound(new { message = result.Message });

            return BadRequest(new { message = result.Message });
        }

        return Ok(new { message = result.Message });
    }


    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);

        if (!result.Success)
        {
            if (result.Message == "Sidebar menu not found.")
                return NotFound(new { message = result.Message });

            return BadRequest(new { message = result.Message });
        }

        return Ok(new { message = result.Message });
    }


    //[HttpGet("my-menu")]
    //public async Task<IActionResult> GetMyMenu()
    //{
    //    var roleIdClaim = User.Claims
    //        .FirstOrDefault(claim => string.Equals(claim.Type, "RoleId", StringComparison.OrdinalIgnoreCase))
    //        ?.Value;

    //    if (!int.TryParse(roleIdClaim, out int roleId))
    //    {
    //        return Unauthorized(new
    //        {
    //            message = "User role information not found in token."
    //        });
    //    }

    //    var menus = await _service.GetMenusByRoleIdAsync(roleId);

    //    return Ok(menus);
    //}
}
