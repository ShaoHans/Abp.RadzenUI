using CRM.Enums;
using Volo.Abp.Application.Dtos;

namespace CRM.Operations;

public class GetAssetsInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }

    public AssetStatus? Status { get; set; }

    public string? Category { get; set; }
}
