using Abp.RadzenUI.Utils;
using Microsoft.AspNetCore.Authorization;
using Radzen;
using Volo.Abp.ObjectExtending;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.Localization;

namespace Abp.RadzenUI.Components.Pages.Tenant;

public partial class List
{
    protected bool HasManageFeaturesPermission;
    protected string ManageFeaturesPolicyName;
    private IReadOnlyList<ExtraPropertyColumnMeta> _extraColumns = default!;

    public List()
    {
        LocalizationResource = typeof(AbpTenantManagementResource);
        ObjectMapperContext = typeof(AbpRadzenUIModule);

        CreatePolicyName = TenantManagementPermissions.Tenants.Create;
        UpdatePolicyName = TenantManagementPermissions.Tenants.Update;
        DeletePolicyName = TenantManagementPermissions.Tenants.Delete;

        ManageFeaturesPolicyName = TenantManagementPermissions.Tenants.ManageFeatures;
    }

    protected override void OnInitialized()
    {
        _extraColumns = RadzenColumnHelper.GetExtraPropertyMetas<TenantDto>();
    }

    protected override Task<TenantUpdateDto> SetEditDialogModelAsync(TenantDto dto)
    {
        var updateDto = new TenantUpdateDto { Name = dto.Name };
        dto.MapExtraPropertiesTo(updateDto);
        return Task.FromResult(updateDto);
    }

    protected override async Task SetPermissionsAsync()
    {
        await base.SetPermissionsAsync();

        HasManageFeaturesPermission = await AuthorizationService.IsGrantedAsync(
            ManageFeaturesPolicyName
        );
    }

    private static DialogOptions SetDialogOptions(int heigth = 450, int width = 700)
    {
        return new DialogOptions
        {
            Draggable = true,
            Width = $"{width}px",
            Height = $"{heigth}px",
        };
    }
}
