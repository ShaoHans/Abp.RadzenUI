using System.ComponentModel.DataAnnotations;

namespace Abp.RadzenUI.Application.Contracts.Organizations;

public class OrganizationUnitAddUserDto
{
    [Required]
    public required List<Guid> UserIds { get; set; }
}

