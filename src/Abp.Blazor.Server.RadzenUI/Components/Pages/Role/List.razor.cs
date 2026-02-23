using Microsoft.AspNetCore.Authorization;
using Radzen;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Localization;

namespace Abp.RadzenUI.Components.Pages.Role;

public partial class List
{
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

    private async Task OpenAssignPermissionDialog(IdentityRoleDto role)
    {
        await DialogService.OpenAsync<Permission>(
            $"{L["Permissions"]} - {role.Name}",
            parameters: new Dictionary<string, object>()
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
}
