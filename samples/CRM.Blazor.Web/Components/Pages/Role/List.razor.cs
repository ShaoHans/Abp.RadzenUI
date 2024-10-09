using Microsoft.AspNetCore.Authorization;
using Radzen;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Localization;

namespace CRM.Blazor.Web.Components.Pages.Role;

public partial class List
{
    protected bool HasManagePermissionsPermission { get; set; }
    protected string ManagePermissionsPolicyName;

    public List()
    {
        ObjectMapperContext = typeof(CRMBlazorWebModule);
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
            IdentityPermissions.Roles.ManagePermissions
        );
    }

    protected override async Task UpdateGetListInputAsync(LoadDataArgs args)
    {
        GetListInput.Filter = _keyword;
        await base.UpdateGetListInputAsync(args);
    }

    protected override Task<IdentityRoleUpdateDto> SetEditDialogModelAsync(IdentityRoleDto dto)
    {
        return Task.FromResult(
            new IdentityRoleUpdateDto
            {
                Name = dto.Name,
                IsDefault = dto.IsDefault,
                IsPublic = dto.IsPublic
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
                { "ProviderKey", role.Name }
            },
            options: new DialogOptions()
            {
                Draggable = true,
                Width = "800px",
                Height = "700px"
            }
        );
    }
}
