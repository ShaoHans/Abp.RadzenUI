namespace Abp.RadzenUI.Application.Contracts.Dashboard;

public class DashboardDto
{
    public int TotalUsers { get; set; }
    public int TotalRoles { get; set; }
    public int TodayAuditLogCount { get; set; }
    public int TodayErrorCount { get; set; }
    public List<AuditLogTrendItem> AuditLogTrend { get; set; } = [];
    public List<HttpStatusCodeStat> HttpStatusCodeStats { get; set; } = [];
    public List<AuditLogTrendItem> ErrorTrend { get; set; } = [];
}

public class AuditLogTrendItem
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
}

public class HttpStatusCodeStat
{
    public string StatusCode { get; set; } = default!;
    public int Count { get; set; }
}
