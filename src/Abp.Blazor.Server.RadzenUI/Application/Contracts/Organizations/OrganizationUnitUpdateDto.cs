using Volo.Abp.ObjectExtending;

namespace Abp.RadzenUI.Application.Contracts.Organizations;

public class OrganizationUnitUpdateDto : ExtensibleObject
{
    public required string DisplayName { get; set; }
}
