using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Abp.RadzenUI.Application.Contracts.Organizations;

public class OrganizationUnitGetChildrenDto : EntityDto<Guid>
{
    public bool Recursive { get; set; }
}

