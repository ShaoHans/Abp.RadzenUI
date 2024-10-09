using Microsoft.AspNetCore.Authorization;
using Radzen;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Localization;

namespace CRM.Blazor.Web.Components.Pages.User;

public partial class List
{
    protected bool HasManagePermissionsPermission { get; set; }
    protected string ManagePermissionsPolicyName;

    public List()
    {
        ObjectMapperContext = typeof(CRMBlazorWebModule);
        LocalizationResource = typeof(IdentityResource);

        CreatePolicyName = IdentityPermissions.Users.Create;
        UpdatePolicyName = IdentityPermissions.Users.Update;
        DeletePolicyName = IdentityPermissions.Users.Delete;
        ManagePermissionsPolicyName = IdentityPermissions.Users.ManagePermissions;
    }

    protected override async Task SetPermissionsAsync()
    {
        await base.SetPermissionsAsync();

        HasManagePermissionsPermission = await AuthorizationService.IsGrantedAsync(
            IdentityPermissions.Users.ManagePermissions
        );
    }

    protected override async Task UpdateGetListInputAsync(LoadDataArgs args)
    {
        GetListInput.Filter = _keyword;
        await base.UpdateGetListInputAsync(args);
    }

    protected override async Task<IdentityUserCreateDto> SetCreateDialogModelAsync()
    {
        var model = await base.SetCreateDialogModelAsync();
        model.IsActive = true;
        model.LockoutEnabled = true;
        model.RoleNames = [];
        return model;
    }

    protected override async Task<IdentityUserUpdateDto> SetEditDialogModelAsync(IdentityUserDto dto)
    {
        var userRoles = (await AppService.GetRolesAsync(dto.Id)).Items?.Select(r => r.Name).ToArray() ?? [];
        return new IdentityUserUpdateDto
        {
            UserName = dto.UserName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            IsActive = dto.IsActive,
            LockoutEnabled = dto.LockoutEnabled,
            RoleNames = userRoles
        };
    }

    private DialogOptions SetDialogOptions()
    {
        return new DialogOptions
        {
            Draggable = true,
            Width = "600px",
            Height = "740px"
        };
    }

    async Task OpenAssignPermissionDialog(IdentityUserDto user)
    {
        await DialogService.OpenAsync<Role.Permission>(
            $"{L["Permissions"]} - {user.UserName}",
            parameters: new Dictionary<string, object>()
            {
                { "ProviderName", "U" },
                { "ProviderKey", user.Id.ToString() }
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
