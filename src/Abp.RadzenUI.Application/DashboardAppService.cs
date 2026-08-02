using Abp.RadzenUI.Application.Contracts.Dashboard;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Application.Services;
using Volo.Abp.AuditLogging;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace Abp.RadzenUI.Application;

public class DashboardAppService : ApplicationService, IDashboardAppService
{
    protected IRepository<AuditLog, Guid> AuditLogRepository { get; }
    protected IIdentityUserRepository UserRepository { get; }
    protected IIdentityRoleRepository RoleRepository { get; }

    public DashboardAppService(
        IRepository<AuditLog, Guid> auditLogRepository,
        IIdentityUserRepository userRepository,
        IIdentityRoleRepository roleRepository)
    {
        AuditLogRepository = auditLogRepository;
        UserRepository = userRepository;
        RoleRepository = roleRepository;
    }

    public virtual async Task<DashboardDto> GetAsync()
    {
        var now = DateTime.Now;
        var todayStart = now.Date.ToUniversalTime();
        var days = 7;
        var trendStart = now.Date.AddDays(-(days - 1)).ToUniversalTime();

        var totalUsers = await UserRepository.GetCountAsync();
        var totalRoles = await RoleRepository.GetCountAsync();

        var auditLogQueryable = await AuditLogRepository.GetQueryableAsync();

        var todayAuditLogCount = await auditLogQueryable
            .CountAsync(x => x.ExecutionTime >= todayStart);

        var todayErrorCount = await auditLogQueryable
            .CountAsync(x => x.ExecutionTime >= todayStart
                && x.HttpStatusCode != null && x.HttpStatusCode >= 400);

        // Audit log trend (last 7 days)
        var trendData = await auditLogQueryable
            .Where(x => x.ExecutionTime >= trendStart)
            .GroupBy(x => x.ExecutionTime.Date)
            .Select(g => new AuditLogTrendItem
            {
                Date = g.Key,
                Count = g.Count()
            })
            .OrderBy(x => x.Date)
            .ToListAsync();

        // Fill missing days
        var auditLogTrend = new List<AuditLogTrendItem>();
        for (var i = 0; i < days; i++)
        {
            var date = now.Date.AddDays(-(days - 1 - i));
            var existing = trendData.FirstOrDefault(x => x.Date.Date == date);
            auditLogTrend.Add(new AuditLogTrendItem
            {
                Date = date,
                Count = existing?.Count ?? 0
            });
        }

        // Error trend (last 7 days)
        var errorTrendData = await auditLogQueryable
            .Where(x => x.ExecutionTime >= trendStart
                && x.HttpStatusCode != null && x.HttpStatusCode >= 400)
            .GroupBy(x => x.ExecutionTime.Date)
            .Select(g => new AuditLogTrendItem
            {
                Date = g.Key,
                Count = g.Count()
            })
            .OrderBy(x => x.Date)
            .ToListAsync();

        var errorTrend = new List<AuditLogTrendItem>();
        for (var i = 0; i < days; i++)
        {
            var date = now.Date.AddDays(-(days - 1 - i));
            var existing = errorTrendData.FirstOrDefault(x => x.Date.Date == date);
            errorTrend.Add(new AuditLogTrendItem
            {
                Date = date,
                Count = existing?.Count ?? 0
            });
        }

        // HTTP status code distribution (last 7 days)
        var statusCodeStats = await auditLogQueryable
            .Where(x => x.ExecutionTime >= trendStart && x.HttpStatusCode != null)
            .GroupBy(x => x.HttpStatusCode!.Value / 100)
            .Select(g => new HttpStatusCodeStat
            {
                StatusCode = (g.Key * 100).ToString(),
                Count = g.Count()
            })
            .OrderBy(x => x.StatusCode)
            .ToListAsync();

        // Format labels
        foreach (var stat in statusCodeStats)
        {
            stat.StatusCode = stat.StatusCode switch
            {
                "200" => "2xx",
                "300" => "3xx",
                "400" => "4xx",
                "500" => "5xx",
                _ => stat.StatusCode + "xx"
            };
        }

        return new DashboardDto
        {
            TotalUsers = (int)totalUsers,
            TotalRoles = (int)totalRoles,
            TodayAuditLogCount = todayAuditLogCount,
            TodayErrorCount = todayErrorCount,
            AuditLogTrend = auditLogTrend,
            ErrorTrend = errorTrend,
            HttpStatusCodeStats = statusCodeStats,
        };
    }
}
