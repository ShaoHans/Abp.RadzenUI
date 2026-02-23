using Volo.Abp.Application.Dtos;

namespace Abp.RadzenUI.Application.Contracts.Organizations;

public class OrganizationUnitGetUnaddedUserListInput : ExtensiblePagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}
