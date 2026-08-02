using Volo.Abp.Application.Dtos;

namespace Abp.RadzenUI.Application.Contracts.CommonDtos;

public class PagedFilterAndSortedResultRequestDto : ExtensiblePagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}
