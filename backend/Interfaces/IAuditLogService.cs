using backend.DTOs;
using backend.DTOs.Pagination;
using backend.Helper;

namespace backend.Interfaces;

public interface IAuditLogService
{
    Task LogAsync(
        int? userId,
        string action,
        string entityName,
        string? entityId = null,
        string? oldValues = null,
        string? newValues = null
    );

    Task<PagedResult<AuditLogDto>> GetLogAsync(
        PaginationRequest request
    );

}
