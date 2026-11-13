

using backend.DTOs;
using backend.DTOs.Pagination;
using backend.Helper;
using backend.Interfaces;
using backend.Model.Database;
using backend.Repo_Interface;

namespace backend.Services;

public class AuditLogService : IAuditLogService
{

    private readonly IAuditLogRepository _repository;

    public AuditLogService(IAuditLogRepository repository)
    {
        _repository = repository;
    }

    public async Task LogAsync(
        int? userId,
        string action,
        string entityName,
        string? entityId = null,
        string? oldValues = null,
        string? newValues = null )
    {
        var auditLog = new AuditLog
        {
            UserId = userId,
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            OldValues = oldValues,
            NewValues = newValues,
            Timestamp = DateTime.UtcNow
        };

        await _repository.AddAsync( auditLog );

    }

    public async Task<PagedResult<AuditLogDto>> GetLogAsync(PaginationRequest request)
    {
        var result = await _repository.GetLogAsync(request);

        return new PagedResult<AuditLogDto>
        {
            Items = result.Items.Select(a => new AuditLogDto
            {
                Id = a.Id,
                UserId = a.UserId,
                UserName = a.User?.FullName,
                Action = a.Action,
                EntityName = a.EntityName,
                EntityId = a.EntityId,
                OldValues = a.OldValues,
                NewValues = a.NewValues,
                Timestamp = a.Timestamp
            }).ToList(),

            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
    }

}
