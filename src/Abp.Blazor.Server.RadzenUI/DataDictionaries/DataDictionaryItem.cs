using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Abp.RadzenUI.DataDictionaries;

public class DataDictionaryItem : AuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    public Guid DataDictionaryTypeId { get; set; }

    public string Code { get; set; } = default!;

    public string Name { get; set; } = default!;

    public int Sort { get; set; }

    public bool IsActive { get; set; } = true;

    public string? Description { get; set; }
}
