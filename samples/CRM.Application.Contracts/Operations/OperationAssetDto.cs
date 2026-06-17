using System;
using CRM.Enums;
using Volo.Abp.Application.Dtos;

namespace CRM.Operations;

public class OperationAssetDto : AuditedEntityDto<Guid>
{
    public string Code { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string Category { get; set; } = default!;

    public string Location { get; set; } = default!;

    public AssetStatus Status { get; set; }

    public int HealthScore { get; set; }

    public DateTime LastInspectionTime { get; set; }
}
