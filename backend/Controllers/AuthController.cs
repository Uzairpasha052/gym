using backend.DTOs;
using backend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _userService;

    public AuthController(IAuthService userService)
    {
        _userService = userService;
    }

    //Register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto request)
    {
        bool created;
        try
        {
            created = await _userService.Register(request);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }

        if (!created)
        {
            return BadRequest(new
            {
                message = "The email already exists or the selected role is invalid."
            });
        }

        return Ok(new
        {
            success = true,
            message = "User Registered."
        });
    }


    //Login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        var user = await _userService.ValidateLogin(request);

        if (user == null)
        {
            return Unauthorized(new
            {
                message = "Invalid Email or Password"
            });
        }

        return Ok(user);
    }

}
