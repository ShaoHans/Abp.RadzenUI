using System.ComponentModel.DataAnnotations;

namespace Abp.RadzenUI.Application.Contracts.Organizations;

public class OrganizationUnitAddRoleDto
{
    [Required]
    public required List<Guid> RoleIds { get; set; }
}

