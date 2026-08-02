using Volo.Abp.Application.Dtos;

namespace Abp.RadzenUI.Application.Contracts.DataDictionaries;

public class GetDataDictionaryTypesInput : ExtensiblePagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}
