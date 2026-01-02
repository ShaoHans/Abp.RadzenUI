using Abp.RadzenUI.Application.Contracts.Organizations;
using Abp.RadzenUI.Localization;
using Abp.RadzenUI.Models;
using Abp.RadzenUI.Permissions;
using Microsoft.AspNetCore.Authorization;
using Radzen;
using Radzen.Blazor;
using Volo.Abp.ObjectExtending;

namespace Abp.RadzenUI.Components.Pages.OrganizationUnit;

public partial class Index
{
    private List<OrganizationUnitTreeItemVm> _ouTree = [];
    private object? _selectedOu;
    public bool HasManageTreePermission { get; set; }
    public bool HasManageUsersPermission { get; set; }
    public bool HasManageRolesPermission { get; set; }

    public Index()
    {
        LocalizationResource = typeof(AbpRadzenUIResource);
    }

    protected override async Task OnInitializedAsync()
    {
        await TrySetPermissionsAsync();
        await LoadOuAsync();
    }

    private async Task TrySetPermissionsAsync()
    {
        if (IsDisposed)
        {
            return;
        }

        await SetPermissionsAsync();
    }

    protected override async Task SetPermissionsAsync()
    {
        HasManageTreePermission = await AuthorizationService.IsGrantedAsync(
            IdentityManagementExtensionPermissions.OrganizationUnits.ManageTree
        );
        HasManageUsersPermission = await AuthorizationService.IsGrantedAsync(
            IdentityManagementExtensionPermissions.OrganizationUnits.ManageUsers
        );
        HasManageRolesPermission = await AuthorizationService.IsGrantedAsync(
            IdentityManagementExtensionPermissions.OrganizationUnits.ManageRoles
        );
    }

    private async Task LoadOuAsync()
    {
        var ous = (await AppService.GetAllAsync()).Items ?? [];
        BuildOuTree(ous);
    }

    protected override Task<OrganizationUnitCreateDto> SetCreateDialogModelAsync()
    {
        var createDto = new OrganizationUnitCreateDto { ParentId = GetSelectedOu()?.Id, };
        return Task.FromResult(createDto);
    }

    protected override Task<OrganizationUnitUpdateDto> SetEditDialogModelAsync(
        OrganizationUnitDto dto
    )
    {
        var updateDto = new OrganizationUnitUpdateDto { DisplayName = dto.DisplayName };
        dto.MapExtraPropertiesTo(updateDto);
        return Task.FromResult(updateDto);
    }

    private void BuildOuTree(IReadOnlyList<OrganizationUnitDto> ous)
    {
        _ouTree = [];
        var ouDict = ous.ToDictionary(
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

    private async Task TreeItemOnClick(RadzenSplitButtonItem item, string buttonName)
    {
        if (item == null)
        {
            return;
        }

        var action = Enum.Parse<OuTreeItemAction>(item.Value!.ToString()!);
        switch (action)
        {
            case OuTreeItemAction.Edit:
                var ou = GetSelectedOu()!;
                await OpenEditDialogAsync<EditOu>(
                    @L["Ou:EditOu.Title", ou.DisplayName],
                    new OrganizationUnitDto
                    {
                        Code = ou.Code,
                        DisplayName = ou.DisplayName,
                        Id = ou.Id,
                        ParentId = ou.ParentId
                    },
                    callback: LoadOuAsync
                );
                break;
            case OuTreeItemAction.AddSubOu:
                await OpenCreateDialogAsync<CreateOu>(L["Ou:NewOu.Title"], callback: LoadOuAsync);
                break;
            case OuTreeItemAction.Delete:
                var selectedOu = GetSelectedOu()!;
                await OpenDeleteConfirmDialogAsync(
                    selectedOu.Id,
                    confirm: L["Ou:DeleteOu.ConfirmMessage", selectedOu.DisplayName],
                    callback: async () =>
                    {
                        _selectedOu = null;
                        await LoadOuAsync();
                    }
                );
                break;
            default:
                break;
        }
    }

    private OrganizationUnitTreeItemVm? GetSelectedOu()
    {
        if (_selectedOu == null)
        {
            return null;
        }
        return (OrganizationUnitTreeItemVm)_selectedOu;
    }
}

public enum OuTreeItemAction
{
    Edit,
    AddSubOu,
    Delete
}
