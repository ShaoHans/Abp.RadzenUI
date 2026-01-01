using Abp.RadzenUI.Application.Contracts.Organizations;
using Abp.RadzenUI.Models;
using Microsoft.AspNetCore.Components;

namespace Abp.RadzenUI.Components.Pages.OrganizationUnit;

public partial class List
{
    [Inject]
    public IOrganizationUnitAppService OuAppService { get; set; } = default!;

    private IReadOnlyList<OrganizationUnitDto> _ous = [];

    private List<OrganizationUnitTreeItemVm> _ouTree = [];

    protected override async Task OnInitializedAsync()
    {
        _ous = (await OuAppService.GetAllAsync()).Items ?? [];
        BuildOuTree();
    }

    private void BuildOuTree()
    {
        var ouDict = _ous.ToDictionary(ou => ou.Id, ou => new OrganizationUnitTreeItemVm
        {
            Id = ou.Id,
            Code = ou.Code,
            DisplayName = ou.DisplayName,
            ParentId = ou.ParentId
        });
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
