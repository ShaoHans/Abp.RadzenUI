using Abp.RadzenUI.Application.Contracts.Organizations;
using Abp.RadzenUI.Localization;
using Abp.RadzenUI.Models;
using Volo.Abp.ObjectExtending;

namespace Abp.RadzenUI.Components.Pages.OrganizationUnit;

public partial class List
{
    public List()
    {
        LocalizationResource = typeof(AbpRadzenUIResource);
    }

    private IReadOnlyList<OrganizationUnitDto> _ous = [];

    private List<OrganizationUnitTreeItemVm> _ouTree = [];

    protected override async Task OnInitializedAsync()
    {
        _ous = (await AppService.GetAllAsync()).Items ?? [];
        BuildOuTree();
    }

    protected override Task<OrganizationUnitUpdateDto> SetEditDialogModelAsync(
        OrganizationUnitDto dto
    )
    {
        var updateDto = new OrganizationUnitUpdateDto { DisplayName = dto.DisplayName, };
        dto.MapExtraPropertiesTo(updateDto);
        return Task.FromResult(updateDto);
    }

    private void BuildOuTree()
    {
        var ouDict = _ous.ToDictionary(
            ou => ou.Id,
            ou => new OrganizationUnitTreeItemVm
            {
                Id = ou.Id,
                Code = ou.Code,
                DisplayName = ou.DisplayName,
                ParentId = ou.ParentId
            }
        );
        foreach (var ou in ouDict.Values)
        {
            if (ou.ParentId.HasValue && ouDict.TryGetValue(ou.ParentId.Value, out var parentOu))
            {
                parentOu.Children.Add(ou);
            }
            else
            {
                _ouTree.Add(ou);
            }
        }
    }
}
