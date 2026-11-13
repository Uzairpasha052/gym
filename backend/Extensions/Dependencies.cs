using backend.Helper;
using backend.Interfaces;
using backend.Jwt;
using backend.Services;
using backend.Services.Interfaces;
using backend.Repository;
using backend.Repo_Interface;

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
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        //Services
        services.AddScoped<IAttendanceService, AttendanceService>();
        services.AddScoped<IAuditLogService, AuditLogService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICashService, CashService>();
        services.AddScoped<IPlayerService, PlayerService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserService, UserService>();
        
        //JWT
        services.AddScoped<IJwtService, JwtService>();

        //Helper
        services.AddScoped<CurrentUserService>();

        services.AddHttpContextAccessor();

        return services;
    }
}
