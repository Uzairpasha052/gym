
using backend.DTOs.Pagination;
using backend.Helper;
using backend.Model.Database;

namespace backend.Repo_Interface;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog auditLog);
    Task<PagedResult<AuditLog>> GetLogAsync(PaginationRequest request);

}
