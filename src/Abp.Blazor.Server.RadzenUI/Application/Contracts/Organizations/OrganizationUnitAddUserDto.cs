using System.ComponentModel.DataAnnotations;

namespace Abp.RadzenUI.Application.Contracts.Organizations;

public class OrganizationUnitAddUserDto
{
    public List<Guid> UserIds { get; set; } = [];
}
