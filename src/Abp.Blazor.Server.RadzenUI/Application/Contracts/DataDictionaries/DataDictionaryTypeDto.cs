using System;
using Volo.Abp.Application.Dtos;

namespace Abp.RadzenUI.Application.Contracts.DataDictionaries;

public class DataDictionaryTypeDto : AuditedEntityDto<Guid>
{
    public string Code { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string? Description { get; set; }
}
