using backend.DTOs.Pagination;
using backend.DTOs.Player;
using backend.Helper;
using backend.Interfaces;
using backend.Models;
using backend.Repo_Interface;
using System.Text.Json;

namespace backend.Services;

public class PlayerService : IPlayerService
{
    private readonly IPlayerRepository _repository;
    private readonly CurrentUserService _currentUser;
    private readonly IAuditLogService _logService;

    public PlayerService(IPlayerRepository repository, CurrentUserService currentUser,
        IAuditLogService logService)
    {
        _repository = repository;
        _currentUser = currentUser;
        _logService = logService;
    }

    public async Task<PagedResult<PlayerResponseDto>> GetAllPlayersAsync(PaginationRequest request)
    {
        //await _repository.GetAllAsync().Select(MapToDto).ToList();
        var players = await _repository.GetAllAsync(request);

        return new PagedResult<PlayerResponseDto>
        {
            Items = players.Items.Select(MapToDto).ToList(),

            TotalCount = players.TotalCount,
            PageNumber = players.PageNumber,
            PageSize = players.PageSize
        };
    }

    public async Task<PlayerResponseDto?> GetPlayerByIdAsync(int id)
    {
        var player = await _repository.GetByIdAsync(id);
        return player is null ? null : MapToDto(player);
    }

    public async Task<PlayerResponseDto> CreatePlayerAsync(CreatePlayerDto dto)
    {
        var player = new Player
        {
            Name = dto.Name,
            FName = dto.FName,
            Age = dto.Age,
            Gender = dto.Gender,
            Contact = dto.Contact,
            History = dto.History,
            Reason = dto.Reason,
            Fee = dto.Fee,
            Status = "Active",
            CreatedBy = _currentUser.UserId
        };

        var createdPlayer = await _repository.CreateAsync(player);

        var newValues = JsonSerializer.Serialize(new
        {
            player.Name,
            player.FName,
            player.Age,
            player.Gender,
            player.Fee,
            player.Status,
            player.IsActive
        });

        await _logService.LogAsync(
            _currentUser.UserId,
            "Create",
            "Player",
            player.Id.ToString(),
            null,
            newValues
        );

        return MapToDto(createdPlayer);
    }

    public async Task<PlayerResponseDto?> UpdatePlayerAsync(int id, UpdatePlayerDto dto)
    {
        var player = await _repository.GetByIdAsync(id);
        if (player is null) return null;

        var oldValues = JsonSerializer.Serialize(new
        {
            player.Name,
            player.FName,
            player.Age,
            player.Gender,
            player.Contact,
            player.History,
            player.Reason,
            player.Fee,
            player.IsActive
        });

        player.Name = dto.Name;
        player.FName = dto.FName;
        player.Age = dto.Age;
        player.Gender = dto.Gender;
        player.Contact = dto.Contact;
        player.History = dto.History;
        player.Reason = dto.Reason;
        player.Fee = dto.Fee;
        player.Status = dto.Status;
        player.UpdatedAt = DateTime.UtcNow;
        player.UpdatedBy = _currentUser.UserId;

        var updated = await _repository.UpdateAsync(player);
        if (!updated) return null;

        var newValues = JsonSerializer.Serialize(new
        {
            player.Name,
            player.FName,
            player.Age,
            player.Gender,
            player.Contact,
            player.History,
            player.Reason,
            player.Fee,
            player.IsActive
        });

        await _logService.LogAsync(
            _currentUser.UserId,
            "Update",
            "Player",
            player.Id.ToString(),
            oldValues,
            newValues
        );

        return MapToDto(player);
    }

    public async Task<bool> DeletePlayerAsync(int id)
    {
        var player = await _repository.GetByIdAsync(id);
        if (player is null) return false;

        var oldValues = JsonSerializer.Serialize(new
        {
            player.Name,
            player.FName,
            player.Age,
            player.Gender,
            player.Fee,
            player.Status,
            player.IsActive,
        });

        player.Status = "InActive";
        player.IsActive = false;
        player.UpdatedAt = DateTime.UtcNow;
        player.UpdatedBy = _currentUser.UserId;

        var update = await _repository.UpdateAsync(player);
        if (!update) return false;

        var newValues = JsonSerializer.Serialize(new
        {
            player.Name,
            player.FName,
            player.Age,
            player.Gender,
            player.Fee,
            player.Status,
            player.IsActive,
        });

        await _logService.LogAsync(
            _currentUser.UserId,
            "Delete",
            "Player",
            player.Id.ToString(),
            oldValues,
            newValues
        );

        return update; 
    }

    private static PlayerResponseDto MapToDto(Player player) => new()
    {
        Id = player.Id,
        Name = player.Name,
        FName = player.FName,
        Age = player.Age,
        Gender = player.Gender,
        Contact = player.Contact,
        History = player.History,
        Reason = player.Reason,
        Fee = player.Fee,
        Status = player.Status,
        CreatedAt = player.CreatedAt
    };
}
