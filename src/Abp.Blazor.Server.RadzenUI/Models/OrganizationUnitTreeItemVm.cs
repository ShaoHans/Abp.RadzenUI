namespace Abp.RadzenUI.Models;

public class OrganizationUnitTreeItemVm
{
    public Guid Id { get; set; }

    public required string Code { get; set; }

    public required string DisplayName { get; set; }

    public Guid? ParentId { get; set; }

    public List<OrganizationUnitTreeItemVm> Children { get; set; } = [];
}
