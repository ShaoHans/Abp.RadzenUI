using System;
using CRM.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace CRM.Operations;

public class OperationAsset : AuditedAggregateRoot<Guid>
{
    public string Code { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string Category { get; set; } = default!;

    public string Location { get; set; } = default!;

    public AssetStatus Status { get; set; }

    public int HealthScore { get; set; }

    public DateTime LastInspectionTime { get; set; }
}
