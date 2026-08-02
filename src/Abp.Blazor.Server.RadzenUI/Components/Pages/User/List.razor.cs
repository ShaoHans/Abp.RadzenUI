using Abp.RadzenUI.Application.Contracts.Users;
using Abp.RadzenUI.Features.Avatar;
using Abp.RadzenUI.Localization;
using Abp.RadzenUI.Infrastructure.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Radzen;
using Volo.Abp.Data;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Localization;
using Volo.Abp.ObjectExtending;

namespace Abp.RadzenUI.Components.Pages.User;

public partial class List
{
    [Inject]
    public IStringLocalizer<AbpRadzenUIResource> IL { get; set; } = default!;

    [Inject]
    public IUserManagementAppService UserManagementAppService { get; set; } = default!;

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
        _extraColumns = RadzenColumnHelper
            .GetExtraPropertyMetas<IdentityUserDto>()
            .Where(x => !x.Name.Equals(AvatarConsts.ExtraPropertyName, StringComparison.Ordinal))
            .ToList();
    }

    protected static string? GetAvatarUrl(IdentityUserDto user)
    {
        var avatarUrl = user.GetProperty<string>(AvatarConsts.ExtraPropertyName);
        return string.IsNullOrWhiteSpace(avatarUrl) ? null : avatarUrl;
    }

    string GetLockoutTitle(IdentityUserDto user)
    {
        if (!user.LockoutEnd.HasValue)
        {
            return string.Empty;
        }

        // A far-future end date (see UserManagementAppService.LockForever) means "locked indefinitely".
        return user.LockoutEnd.Value.UtcDateTime > DateTime.UtcNow.AddYears(100)
            ? IL["User:LockedPermanently"]
            : IL["User:LockedUntil", user.LockoutEnd.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")];
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
            Surname = dto.Surname,
            Name = dto.Name,            
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
            parameters: new Dictionary<string, object?>()
            {
                { "ProviderName", "U" },
                { "ProviderKey", user.Id.ToString() },
            },
            options: new DialogOptions() { Draggable = true, Width = "800px", }
        );
    }

    async Task OpenPermissionVisualizerDialog(IdentityUserDto user)
    {
        await DialogService.OpenAsync<Abp.RadzenUI.Components.Pages.Permission.Visualizer>(
            $"{IL["PermissionVisualizer:Title"]} - {user.UserName}",
            parameters: new Dictionary<string, object?>()
            {
                { "ProviderName", "U" },
                { "ProviderKey", user.Id.ToString() },
            },
            options: new DialogOptions() { Draggable = true, Width = "1000px", }
        );
    }

    async Task OpenEditUserAsync(IdentityUserDto user)
    {
        await OpenEditDialogAsync<Edit>(
            L["Edit"],
            user,
            SetDialogOptions,
            new Dictionary<string, object?> { { "UserId", user.Id } }
        );
    }

    async Task DeleteUserAsync(IdentityUserDto user)
    {
        await OpenDeleteConfirmDialogAsync(
            user.Id,
            L["Delete"],
            L["UserDeletionConfirmationMessage", user.UserName]
        );
    }

    async Task LockUserAsync(IdentityUserDto user)
    {
        var result = await DialogService.OpenAsync<LockUser>(
            $"{IL["User:Lock"]} - {user.UserName}",
            new Dictionary<string, object?>
            {
                { "UserId", user.Id },
                { "UserName", user.UserName },
            },
            new DialogOptions { Draggable = true, Width = "460px" }
        );

        if (result is true)
        {
            await _grid.Reload();
        }
    }

    async Task UnlockUserAsync(IdentityUserDto user)
    {
        try
        {
            await UserManagementAppService.UnlockAsync(user.Id);
            await Notify.Success(IL["User:UnlockSuccess", user.UserName]);
            await _grid.Reload();
        }
        catch (Exception ex)
        {
            await Notify.Error(ex.Message);
        }
    }

    async Task OpenSetPasswordDialogAsync(IdentityUserDto user)
    {
        await DialogService.OpenAsync<SetPassword>(
            $"{IL["User:SetPassword"]} - {user.UserName}",
            new Dictionary<string, object?> { { "UserId", user.Id } },
            SetDialogOptions()
        );
    }
}
