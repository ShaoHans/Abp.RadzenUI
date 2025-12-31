using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Abp.RadzenUI.Application.Contracts.Organizations;

public class OrganizationUnitGetChildrenDto : IEntityDto<Guid>
{
    [Required]
    public Guid Id { get; set; }

    public bool Recursive { get; set; }
}

