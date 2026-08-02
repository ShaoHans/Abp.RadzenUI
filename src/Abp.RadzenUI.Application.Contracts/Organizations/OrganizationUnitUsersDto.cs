using Volo.Abp.Identity;

namespace Abp.RadzenUI.Application.Contracts.Organizations;

public class OrganizationUnitUsersDto : OrganizationUnitDto
{
    public required ICollection<IdentityUserDto> Users { get; set; }
}
