using backend.DTOs.Pagination;
using backend.DTOs.Player;
using backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[Route("api/player")]
[ApiController]
[Authorize]
public class PlayerController : ControllerBase
{
    private readonly IPlayerService _playerService;

    public PlayerController(IPlayerService playerService)
    {
        _playerService = playerService;
    }
 
    [HttpGet]
    public async Task<IActionResult> GetAllPlayers([FromQuery] PaginationRequest request)
    {
        var players = await _playerService.GetAllPlayersAsync(request);
        return Ok(players);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPlayerById(int id)
    {
        var player = await _playerService.GetPlayerByIdAsync(id);

        if (player == null)
            return NotFound(new { message = "Player not found." });

        return Ok(player);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePlayer(CreatePlayerDto dto)
    {
        var player = await _playerService.CreatePlayerAsync(dto);
        return Ok(player);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePlayer(int id, UpdatePlayerDto dto)
    {
        var player = await _playerService.UpdatePlayerAsync(id, dto);

        if (player == null)
            return NotFound(new { message = "Player not found." });

        return Ok(player);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlayer(int id)
    {
        var deleted = await _playerService.DeletePlayerAsync(id);

        if (!deleted)
            return NotFound(new { message = "Player not found." });

        return Ok(new { message = "Player InActive successfully." });
    }
}