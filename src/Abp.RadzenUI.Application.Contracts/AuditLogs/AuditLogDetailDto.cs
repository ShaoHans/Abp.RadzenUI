namespace Abp.RadzenUI.Application.Contracts.AuditLogs;

public class AuditLogDetailDto : AuditLogDto
{
    public List<AuditLogActionDto> Actions { get; set; } = [];

    public List<EntityChangeDto> EntityChanges { get; set; } = [];
}
