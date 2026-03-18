using System;
using Volo.Abp.Application.Dtos;

namespace Abp.RadzenUI.Application.Contracts.DataDictionaries;

public class GetDataDictionaryItemsInput : ExtensiblePagedAndSortedResultRequestDto
{
    public Guid DataDictionaryTypeId { get; set; }

    public string? Filter { get; set; }
}
