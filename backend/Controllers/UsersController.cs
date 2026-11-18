using Microsoft.AspNetCore.Mvc;
using backend.Interfaces;
//using Microsoft.AspNetCore.Identity.Data;
using backend.DTOs;
using Microsoft.AspNetCore.Authorization;
using backend.DTOs.Pagination;

namespace backend.Controllers;

// [Authorize]
[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;        
    }

    //get Users 
    [Authorize]
    [HttpGet("allUsers")]
    public async Task<IActionResult> GetAllUsers([FromQuery] PaginationRequest request)
    {
        var users = await _userService.GetAllUsersAsync(request);

        return Ok(users);
    }

    //delete User
    [Authorize]
    [HttpDelete("deleteUser/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var result = await _userService.DeleteUser(id);

        if(!result)
            return NotFound();
        
        return NoContent();
    }

    //Update User
    [Authorize]
    [HttpPut("updateUser/{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
    {
        backend.DTOs.UserDto? user;
        try { user = await _userService.UpdateUser(id, dto); }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }

        if(user == null)
            return NotFound();

        return Ok(user);
    }

}
