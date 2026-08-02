using Volo.Abp.Auditing;

namespace Abp.RadzenUI.Application.Contracts.AuditLogs;

public class EntityChangeDto
{
    public Guid Id { get; set; }

    public DateTime ChangeTime { get; set; }

    public EntityChangeType ChangeType { get; set; }

    public string? EntityId { get; set; }

    public string? EntityTypeFullName { get; set; }

    public List<EntityPropertyChangeDto> PropertyChanges { get; set; } = [];
}
