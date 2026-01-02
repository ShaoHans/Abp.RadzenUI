using Abp.RadzenUI.Localization;
using Abp.RadzenUI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Radzen;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Localization;
using Volo.Abp.ObjectExtending;

namespace Abp.RadzenUI.Components.Pages.User;

public partial class List
{
    [Inject]
    public IStringLocalizer<AbpRadzenUIResource> IL { get; set; } = default!;
    protected bool HasManagePermissionsPermission { get; set; }
    protected string ManagePermissionsPolicyName;
    private IReadOnlyList<ExtraPropertyColumnMeta> _extraColumns = default!;

    public List()
    {
        ObjectMapperContext = typeof(AbpRadzenUIModule);
        LocalizationResource = typeof(IdentityResource);

        CreatePolicyName = IdentityPermissions.Users.Create;
        UpdatePolicyName = IdentityPermissions.Users.Update;
        DeletePolicyName = IdentityPermissions.Users.Delete;
        ManagePermissionsPolicyName = IdentityPermissions.Users.ManagePermissions;
    }

    protected override void OnInitialized()
    {
        _extraColumns = RadzenColumnHelper.GetExtraPropertyMetas<IdentityUserDto>();
    }

    protected override async Task SetPermissionsAsync()
    {
        await base.SetPermissionsAsync();

        HasManagePermissionsPermission = await AuthorizationService.IsGrantedAsync(
            ManagePermissionsPolicyName
        );
    }

    protected override async Task<IdentityUserCreateDto> SetCreateDialogModelAsync()
    {
        var model = await base.SetCreateDialogModelAsync();
        model.IsActive = true;
        model.LockoutEnabled = true;
        model.RoleNames = [];
        return model;
    }

    protected override async Task<IdentityUserUpdateDto> SetEditDialogModelAsync(
        IdentityUserDto dto
    )
    {
        var userRoles =
            (await AppService.GetRolesAsync(dto.Id)).Items?.Select(r => r.Name).ToArray() ?? [];
        var updateDto = new IdentityUserUpdateDto
        {
            UserName = dto.UserName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            IsActive = dto.IsActive,
            LockoutEnabled = dto.LockoutEnabled,
            RoleNames = userRoles,
        };

        dto.MapExtraPropertiesTo(updateDto);

        return updateDto;
    }

    private DialogOptions SetDialogOptions()
    {
        return new DialogOptions
        {
            Draggable = true,
            Width = "600px",
        };
    }

    async Task OpenAssignPermissionDialog(IdentityUserDto user)
    {
        await DialogService.OpenAsync<Role.Permission>(
            $"{L["Permissions"]} - {user.UserName}",
            parameters: new Dictionary<string, object>()
            {
                { "ProviderName", "U" },
                { "ProviderKey", user.Id.ToString() },
            },
            options: new DialogOptions() { Draggable = true, Width = "800px", }
        );
    }
}
