using System;
using CRM.Enums;
using Volo.Abp.Application.Dtos;

namespace CRM.Operations;

public class WorkOrderDto : AuditedEntityDto<Guid>
{
    public string Code { get; set; } = default!;

    public string Title { get; set; } = default!;

    public WorkOrderType Type { get; set; }

    public WorkOrderStatus Status { get; set; }

    public OperationPriority Priority { get; set; }

    public string OwnerName { get; set; } = default!;

    public string Location { get; set; } = default!;

    public DateTime PlannedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public string? Description { get; set; }
}
