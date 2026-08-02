using System;
using Volo.Abp.Application.Dtos;

namespace Abp.RadzenUI.Application.Contracts.DataDictionaries;

public class DataDictionaryItemDto : AuditedEntityDto<Guid>
{
    public Guid DataDictionaryTypeId { get; set; }

    public string Code { get; set; } = default!;

    public string Name { get; set; } = default!;

    public int Sort { get; set; }

    public bool IsActive { get; set; }

    public string? Description { get; set; }
}
