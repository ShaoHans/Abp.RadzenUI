namespace Abp.RadzenUI.Application.Contracts.AuditLogs;

public class AuditLogActionDto
{
    public string? ServiceName { get; set; }

    public string? MethodName { get; set; }

    public string? Parameters { get; set; }

    public DateTime ExecutionTime { get; set; }

    public int ExecutionDuration { get; set; }
}
