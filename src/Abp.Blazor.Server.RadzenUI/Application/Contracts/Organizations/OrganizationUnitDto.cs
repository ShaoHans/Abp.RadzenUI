using Volo.Abp.Application.Dtos;

namespace Abp.RadzenUI.Application.Contracts.Organizations;

public class OrganizationUnitDto : ExtensibleFullAuditedEntityDto<Guid>
{
    public Guid? ParentId { get; set; }

    public required string Code { get; set; }

    public required string DisplayName { get; set; }
}
