using Microsoft.AspNetCore.Authorization;
using Radzen;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.Localization;

namespace Abp.RadzenUI.Components.Pages.Tenant;

public partial class List
{
    protected bool HasManageFeaturesPermission;
    protected string ManageFeaturesPolicyName;

    public List()
    {
        LocalizationResource = typeof(AbpTenantManagementResource);
        ObjectMapperContext = typeof(AbpRadzenUIModule);

        CreatePolicyName = TenantManagementPermissions.Tenants.Create;
        UpdatePolicyName = TenantManagementPermissions.Tenants.Update;
        DeletePolicyName = TenantManagementPermissions.Tenants.Delete;

        ManageFeaturesPolicyName = TenantManagementPermissions.Tenants.ManageFeatures;
    }

    protected override Task<TenantUpdateDto> SetEditDialogModelAsync(TenantDto dto)
    {
        throw new NotImplementedException();
    }

    protected override async Task SetPermissionsAsync()
    {
        await base.SetPermissionsAsync();

        HasManageFeaturesPermission = await AuthorizationService.IsGrantedAsync(
            ManageFeaturesPolicyName
        );
    }

    protected override async Task UpdateGetListInputAsync(LoadDataArgs args)
    {
        GetListInput.Filter = _keyword;
        await base.UpdateGetListInputAsync(args);
    }

    //protected override async Task<IdentityUserCreateDto> SetCreateDialogModelAsync()
    //{
    //    var model = await base.SetCreateDialogModelAsync();
    //    model.IsActive = true;
    //    model.LockoutEnabled = true;
    //    model.RoleNames = [];
    //    return model;
    //}

    //protected override async Task<IdentityUserUpdateDto> SetEditDialogModelAsync(
    //    IdentityUserDto dto
    //)
    //{
    //    var userRoles =
    //        (await AppService.GetRolesAsync(dto.Id)).Items?.Select(r => r.Name).ToArray() ?? [];
    //    return new IdentityUserUpdateDto
    //    {
    //        UserName = dto.UserName,
    //        Email = dto.Email,
    //        PhoneNumber = dto.PhoneNumber,
    //        IsActive = dto.IsActive,
    //        LockoutEnabled = dto.LockoutEnabled,
    //        RoleNames = userRoles
    //    };
    //}

    private DialogOptions SetDialogOptions()
    {
        return new DialogOptions
        {
            Draggable = true,
            Width = "700px",
            Height = "450px"
        };
    }
}
