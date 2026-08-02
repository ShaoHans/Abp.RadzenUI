using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Abp.RadzenUI.Messages;

public class UserMessage : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    public Guid UserId { get; set; }

    public string Title { get; set; } = default!;

    public string Content { get; set; } = default!;

    public string MessageType { get; set; } = default!;

    public bool IsRead { get; set; }

    public DateTime? ReadTime { get; set; }
}