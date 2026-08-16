using Abp.RadzenUI.Components.Shared;
using Abp.RadzenUI.Localization;
using Microsoft.AspNetCore.Authorization;
using Radzen;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Localization;

namespace Abp.RadzenUI.Components.Pages.Role;

public partial class List
{
    private const string EditKey = "edit";
    private const string ViewPermissionsKey = "view-permissions";
    private const string ManagePermissionsKey = "manage-permissions";
    private const string ClaimsKey = "claims";
    private const string DeleteKey = "delete";

    protected bool HasManagePermissionsPermission { get; set; }
    protected string ManagePermissionsPolicyName;

    public List()
    {
        ObjectMapperContext = typeof(AbpRadzenUIModule);
        LocalizationResource = typeof(IdentityResource);

        CreatePolicyName = IdentityPermissions.Roles.Create;
        UpdatePolicyName = IdentityPermissions.Roles.Update;
        DeletePolicyName = IdentityPermissions.Roles.Delete;
        ManagePermissionsPolicyName = IdentityPermissions.Roles.ManagePermissions;
    }

    protected override async Task SetPermissionsAsync()
    {
        await base.SetPermissionsAsync();

        HasManagePermissionsPermission = await AuthorizationService.IsGrantedAsync(
            ManagePermissionsPolicyName
        );
    }

    protected override Task<IdentityRoleUpdateDto> SetEditDialogModelAsync(IdentityRoleDto dto)
    {
        return Task.FromResult(
            new IdentityRoleUpdateDto
            {
                Name = dto.Name,
                IsDefault = dto.IsDefault,
                IsPublic = dto.IsPublic,
            }
        );
    }

    /// <summary>
    /// Actions offered for a role row, in display order. The "admin" role stays
    /// editable only for the admin user itself, and is never deletable.
    /// </summary>
    private IReadOnlyList<RowAction> BuildRowActions(IdentityRoleDto role)
    {
        var isProtectedAdmin = role.Name == "admin" && CurrentUser.UserName != "admin";
        var actions = new List<RowAction>();

        if (!isProtectedAdmin && HasUpdatePermission)
        {
            actions.Add(new RowAction(EditKey, L["Edit"], "edit", IsPrimary: true));
        }

        if (!isProtectedAdmin && HasManagePermissionsPermission)
        {
            actions.Add(
                new RowAction(ViewPermissionsKey, UL["PermissionVisualizer:Title"], "visibility")
            );
            actions.Add(new RowAction(ManagePermissionsKey, L["Permissions"], "productivity"));
        }

        if (!isProtectedAdmin && HasUpdatePermission)
        {
            actions.Add(new RowAction(ClaimsKey, UL["RoleClaim:Claims"], "badge"));
        }

        if (role.Name != "admin" && HasDeletePermission)
        {
            actions.Add(new RowAction(DeleteKey, L["Delete"], "delete", Colors.Danger));
        }

        return actions;
    }

    private Task HandleRowActionAsync(string key, IdentityRoleDto role)
    {
        return key switch
        {
            EditKey => OpenEditDialogAsync<Edit>(L["Edit"], role),
            ViewPermissionsKey => OpenPermissionVisualizerDialog(role),
            ManagePermissionsKey => OpenAssignPermissionDialog(role),
            ClaimsKey => OpenClaimsDialog(role),
            DeleteKey => OpenDeleteConfirmDialogAsync(
                role.Id,
                L["Delete"],
                L["RoleDeletionConfirmationMessage", role.Name]
            ),
            _ => Task.CompletedTask,
        };
    }

    private async Task OpenAssignPermissionDialog(IdentityRoleDto role)
    {
        await DialogService.OpenAsync<Permission>(
            $"{L["Permissions"]} - {role.Name}",
            parameters: new Dictionary<string, object?>()
            {
                { "ProviderName", "R" },
                { "ProviderKey", role.Name },
            },
            options: new DialogOptions()
            {
                Draggable = true,
                Width = "800px",
            }
        );
    }

    private async Task OpenPermissionVisualizerDialog(IdentityRoleDto role)
    {
        await DialogService.OpenAsync<Abp.RadzenUI.Components.Pages.Permission.Visualizer>(
            $"{UL["PermissionVisualizer:Title"]} - {role.Name}",
            parameters: new Dictionary<string, object?>()
            {
                { "ProviderName", "R" },
                { "ProviderKey", role.Name },
            },
            options: new DialogOptions()
            {
                Draggable = true,
                Width = "1000px",
            }
        );
    }

    private async Task OpenClaimsDialog(IdentityRoleDto role)
    {
        await DialogService.OpenAsync<Claims>(
            $"{UL["RoleClaim:Claims"]} - {role.Name}",
            parameters: new Dictionary<string, object?>()
            {
                { "RoleId", role.Id },
            },
            options: new DialogOptions()
            {
                Draggable = true,
                Width = "700px",
            }
        );
    }
}
