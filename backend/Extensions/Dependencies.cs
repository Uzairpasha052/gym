using backend.Helper;
using backend.Interface;
using backend.Interfaces;
using backend.Jwt;
using backend.Repo_Interface;
using backend.Repository;
using backend.Service;
using backend.Services;
using backend.Services.Interfaces;

namespace backend.Extensions;

public static class Dependencies
{
    public static IServiceCollection AddApplicationDependencies(
        this IServiceCollection services)
    {
        //Repositries
        services.AddScoped<IAttendanceRepository, AttendanceRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<ICashRepository, CashRepository>();
        services.AddScoped<IMembershipRepository, MembershipRepository>();
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<ISidebarRepository, SidebarRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        //Services
        services.AddScoped<IAttendanceService, AttendanceService>();
        services.AddScoped<IAuditLogService, AuditLogService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICashService, CashService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IMembershipService, MembershipService>();
        services.AddScoped<IPlayerService, PlayerService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<ISidebarService, SidebarService>();
        services.AddScoped<IUserService, UserService>();
        
        //JWT
        services.AddScoped<IJwtService, JwtService>();

        //Helper
        services.AddScoped<CurrentUserService>();

        services.AddHttpContextAccessor();

        return services;
    }
}
