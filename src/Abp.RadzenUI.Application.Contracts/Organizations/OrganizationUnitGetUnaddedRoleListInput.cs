using Volo.Abp.Application.Dtos;

namespace Abp.RadzenUI.Application.Contracts.Organizations;

public class OrganizationUnitGetUnaddedRoleListInput : ExtensiblePagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}
