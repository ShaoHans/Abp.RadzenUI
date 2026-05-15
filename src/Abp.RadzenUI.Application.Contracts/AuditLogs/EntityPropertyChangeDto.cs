namespace Abp.RadzenUI.Application.Contracts.AuditLogs;

public class EntityPropertyChangeDto
{
    public string? PropertyName { get; set; }

    public string? PropertyTypeFullName { get; set; }

    public string? OriginalValue { get; set; }

    public string? NewValue { get; set; }
}
