using backend.DTOs.Dashboard;

namespace backend.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetAdminAsync();
    Task<DashboardSummaryDto> GetReceptionistAsync();
    Task<DashboardSummaryDto?> GetMemberAsync(int userId);
}
